using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*VERSION: 0.05*/
public class SmartAnimator : MonoBehaviour
{
    [Header("Main:")]
    public SpriteRenderer spRenderer;
    public Image imageRenderer;

    public float _speed = 1f;
    public float speed
    {
        get
        {
            return _speed;
        }
        set
        {
            _speed = value;
            if (_speed < 0) _speed = 0;
        }
    }

    public bool isExitAnimationBlocked;
    public bool unscaledTimeUpdate;
    public bool destroyAfterPlay;
    public bool destroyParent;

    [Header("Animations: ")]
    public SmartAnimation[] animations;

    private int currentAnimationID;
    private int currentFrameID = 0;
    private float currentFrameTime;

    public int ID
    {
        get
        {
            return currentAnimationID;
        }
    }

    public void Awake()
    {
        imageRenderer = GetComponent<Image>();
        spRenderer = GetComponent<SpriteRenderer>();

        if ((spRenderer == null && imageRenderer == null) || animations == null) Destroy(this);
        currentFrameTime = animations[currentAnimationID].frameTime;
    }

    private bool isEnded;

    private void SetSprite (Sprite sprite)
    {
        if (spRenderer != null) spRenderer.sprite = sprite;
        if (imageRenderer != null) imageRenderer.sprite = sprite;
    }

    void Update()
    {
        if (isEnded) return;

        if (unscaledTimeUpdate)
            currentFrameTime -= Time.unscaledDeltaTime * speed;
        else
            currentFrameTime -= Time.deltaTime * speed;

        if (currentFrameTime <= 0)
        {
            currentFrameTime = animations[currentAnimationID].frameTime;

            int nextIndex = currentFrameID + 1;
            if (nextIndex >= animations[currentAnimationID].sprites.Count) nextIndex = 0;
            if (!animations[currentAnimationID].circled && nextIndex == 0)
            {
                isEnded = true;
                if (animations[currentAnimationID].exitAnimation && isExitAnimationBlocked == false)
                {
                    SetAnimation(animations[currentAnimationID].exitAnimationID);
                }
                else
                {
                    if (destroyAfterPlay)
                    {
                        if (destroyParent) Destroy(this.transform.parent.gameObject);
                        else Destroy(this.gameObject);
                    }
                }
                return;
            }

            if (animations[currentAnimationID].sprites != null && animations[currentAnimationID].sprites.Count > 0)
            {
                SetSprite(animations[currentAnimationID].sprites[nextIndex]);
            }
            //spRenderer.sprite = animations[currentAnimationID].sprites[nextIndex];
            currentFrameID = nextIndex;
        }
    }

    public void SetAnimation (int ID)
    {
        if (ID < 0 || ID >= animations.Length) return;
        if (spRenderer == null && imageRenderer == null) return;

        currentAnimationID = ID;
        currentFrameID = 0;

        if (animations[currentAnimationID].sprites != null && animations[currentAnimationID].sprites.Count > 0)
        {
            SetSprite(animations[currentAnimationID].sprites[currentFrameID]);
        }

        currentFrameTime = animations[currentAnimationID].frameTime;
        isEnded = false;
    }

    [System.Serializable]
    public class SmartAnimation
    {
#if UNITY_EDITOR
        public string editorTitle;
        public bool isVisible;
#endif
        [Header("Main: ")]
        public float frameTime = 0.1f;
        public List<Sprite> sprites;
        public bool circled;

        [Header("Exit: ")]
        public bool exitAnimation;
        public int exitAnimationID;
    }
}
