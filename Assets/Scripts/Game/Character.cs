using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{
    public static Character Active { get; private set; }

    public CameraFollower CameraFollower;
    public SpriteBlinks ImmortalBlinks;
    public GameObject CharacterDebris;
    public GroundCheck GroundCheck;
    public SaveScore SaveScorePrefab;
    public GameObject SaveScoreDemo;
    public ParticleSystem FootstepsParticles;

    [Header("Sounds:")]
    public SoundSettings DeathSound;
    public SoundSettings ThrowSound;
    public SoundSettings GroundedSound;

    public SoundSettings RunSound;
    public AudioSource RunSoundSource;
    private float _footstepTime = 0.35f;
    private float _footstepDelay;

    [Header("Values:")]
    public float ThrowCooldown = 1f;
    public float ThrowAnimationTime = 1f;
    public float ThrowThingSpawnDelay = 0.35f;
    public float JumpForce = 10f;
    private bool _isGrounded;

    public ThrowableItem[] Items;

    public bool IsLookingRight => !UnitRenderer.flipX;

    private float _throwCooldown;
    private float _animatorCooldown;

    private float _freezePhysicsTime;
    private float _immortalityTime;
    private float _saveScoreDemoTime;

    private float _motionLockDelay;
    private Vector2 _targetValocity;

    [System.Serializable]
    public struct ThrowableItem
    {
        public Item Item;
        public float Chance;
    }

    public void Awake()
    {
        Active = this;
        RunSoundSource = SoundManager.PlaySource(RunSound, RunSoundSource);
    }

    public void FixedUpdate()
    {
        if (_freezePhysicsTime > 0)
        {
            UnitPhysics.simulated = false;
        }
        else
        {
            UnitPhysics.simulated = true;
            _targetValocity.x = Mathf.Lerp(_targetValocity.x, 0f, Time.fixedDeltaTime * 8f);
            UnitPhysics.velocity = _targetValocity;
        }
    }

    public void Update()
    {
        _throwCooldown -= Time.deltaTime;
        _animatorCooldown -= Time.deltaTime;
        _freezePhysicsTime -= Time.deltaTime;
        _immortalityTime -= Time.deltaTime;
        _targetValocity = UnitPhysics.velocity;
        _motionLockDelay -= Time.deltaTime;
        _saveScoreDemoTime -= Time.deltaTime;
        _footstepDelay -= Time.deltaTime;

        SaveScoreDemo.SetActive(_saveScoreDemoTime > 0);

        ImmortalBlinks.IsBlinking = _immortalityTime > 0;

        if(_isGrounded == false && GroundCheck.isGrounded)
        {
            SoundManager.PlayWorldSpace(GroundedSound, transform.position);
        }

        _isGrounded = GroundCheck.isGrounded;

        if (_animatorCooldown <= 0 && _throwCooldown <= 0f)
        {
            if (Input.GetMouseButton(0))
            {
                _throwCooldown = ThrowCooldown;

                if (GroundCheck.isGrounded)
                {
                    SetAnimatorState(AnimatorState.Throw, ThrowAnimationTime);
                }
                else
                {
                    SetAnimatorState(AnimatorState.ThrowFall, ThrowAnimationTime);
                }

                Invoke(nameof(ThrowSomething), ThrowThingSpawnDelay);
                return;
            }
        }

        if(_isGrounded && Mathf.Abs(UnitPhysics.velocity.x) > 0.25f)
        {
            FootstepsParticles.transform.localEulerAngles = new Vector3(0, 0, UnitPhysics.velocity.x < 0 ? -45f : 45f);
            FootstepsParticles.transform.localPosition = new Vector3(UnitPhysics.velocity.x < 0 ? 0.15f : -0.15f, 0, 0);
            
            if (FootstepsParticles.loop == false)
            {
                FootstepsParticles.loop = true;
                FootstepsParticles.Play();
            }
        }
        else
        {
            FootstepsParticles.loop = false;
        }

        if (_motionLockDelay < 0)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                _targetValocity.x = -1f * MotionSpeed;
            }
            else
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                _targetValocity.x = 1f * MotionSpeed;
            }
            else
            {
                _targetValocity.x = 0f;
            }

            if (Input.GetKey(KeyCode.Space) && GroundCheck.isGrounded)
            {
                _targetValocity.y = JumpForce;
            }

            bool isTurnedLeft = IsTurnedLeft();
            float animationSpeed = UnitPhysics.velocity.x / MotionSpeed;

            UnitRenderer.flipX = isTurnedLeft;
            UnitAnimator.SetBool("Inverted", isTurnedLeft);
            UnitAnimator.SetFloat("Speed", animationSpeed);
        }

        if (_animatorCooldown < 0)
        {
            if (GroundCheck.isGrounded)
            {
                if (Mathf.Abs(UnitPhysics.velocity.x) > 0.1f)
                {
                    State = AnimatorState.Run;
                    if(_footstepDelay <= 0)
                    {
                        _footstepDelay = _footstepTime;
                        RunSoundSource.Play();
                    }
                }
                else
                {
                    State = AnimatorState.Idle;
                }
            }
            else
            {
                if (UnitPhysics.velocity.y > 0.1f)
                {
                    State = AnimatorState.Jump;
                }
                else
                if (UnitPhysics.velocity.y < 0.1f)
                {
                    State = AnimatorState.Fall;
                }
            }
        }
    }

    public void SetAnimatorState (AnimatorState animatorState, float animationDelay)
    {
        _animatorCooldown = animationDelay;
        ForcedAnimation(animatorState);
    }

    public void SetAnimatorState (AnimatorState animatorState, float animationDelay, float motionLockDelay)
    {
        _animatorCooldown = animationDelay;
        _motionLockDelay = motionLockDelay;
        ForcedAnimation(animatorState);
    }

    public void SetPhysicsFreezing (float time)
    {
        _freezePhysicsTime = time;
    }

    public void SetMotionLockDelay (float delay)
    {
        _motionLockDelay = delay;
    }

    public void ShowSaveScoreDemo (float time)
    {
        _saveScoreDemoTime = time;
    }
    
    public void SetImmortality (float time)
    {
        _immortalityTime = time;
    }

    public void DieAndRestart (bool isForced = false)
    {
        if (!isForced && _immortalityTime > 0)
        {
            return;
        }

        Bug[] bugs = FindObjectsOfType<Bug>(true);
        for (int i = 0; i < bugs.Length; i++)
        {
            bugs[i].SetPhysicsFreezing(1f);
        }

        GameRules.BakeResults();

        CameraFollower.Shock(0.2f);
        UnitPhysics.velocity = _targetValocity = Vector2.zero;

        GameObject debris = GameObject.Instantiate(CharacterDebris);
        debris.transform.position = transform.position;
        SoundManager.PlayWorldSpace(DeathSound, transform.position);

        Vector2 spawnPoint = transform.position;
        int spawnersCount = GameRules.Instance.CharacterSpawners.Length;
        spawnPoint = GameRules.Instance.CharacterSpawners[Random.Range(0, spawnersCount)].position;

        SetImmortality(2f);
        Vector2 prevPosition = transform.position;
        transform.position = spawnPoint;
        
        SaveScore saveScore = SaveScore.Instantiate(SaveScorePrefab);
        saveScore.transform.position = prevPosition + Vector2.up * 0.65f;

        saveScore.SavedScore = GameRules.Score;
        GameRules.ChangeScore(-GameRules.Score);

        FullCover.Instance.CoverTime = 0.5f;
        SetAnimatorState(AnimatorState.Restart, 1f, 1f);
        GameRules.ChangeSeed();
    }

    public void ThrowSomething()
    {
        Item item = GetRandomItem();
        item = Item.Instantiate(item);
        SoundManager.PlayWorldSpace(ThrowSound, transform.position);

        Vector3 offset = (IsTurnedLeft() == true ? Vector2.left : Vector2.right) * 0.25f;
        Vector3 position = transform.position + offset + (Vector3.up * 0.45f);
        item.transform.position = position;

        Vector2 mousePosition = Input.mousePosition;
        Vector3 target = Camera.main.ScreenToWorldPoint(mousePosition);

        item.Throw(target - position);
    }

    private Item GetRandomItem ()
    {
        float randomSum = 0;
        for(int i = 0; i < Items.Length; i++)
        {
            randomSum += Items[i].Chance;
        }
        randomSum = Random.Range(0, randomSum);

        float value = 0;
        for (int i = 0; i < Items.Length; i++)
        {
            value += Items[i].Chance;
            if(value >= randomSum)
            {
                return Items[i].Item;
            }
        }
        return Items[0].Item;
    }

    protected bool IsTurnedLeft ()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 target = Camera.main.ScreenToWorldPoint(mousePosition);
        return target.x < transform.position.x;
    }
}
