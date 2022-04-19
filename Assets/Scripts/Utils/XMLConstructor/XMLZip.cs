using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class XMLZip
{
    public string Item { get; private set; }
    public string Value { get; private set; }
    public List<XMLZip> InnerZip { get; private set; }

    public XMLZip(string item)
    {
        Item = item;
        InnerZip = new List<XMLZip>();
    }

    public static XMLZip Zip(string item, string value)
    {
        XMLZip zip = new XMLZip(item);
        zip.Value = value;
        return zip;
    }

    public static XMLZip Zip(string item, List<XMLZip> innerZip)
    {
        XMLZip zip = new XMLZip(item);
        zip.InnerZip.AddRange(innerZip);
        return zip;
    }

    public static XMLZip Zip(string item, params XMLZip[] innerZip)
    {
        XMLZip zip = new XMLZip(item);
        zip.InnerZip.AddRange(innerZip);
        return zip;
    }

    public void SetValue (string value)
    {
        Value = value;
    }

    public void Add(params XMLZip[] innerZip)
    {
        InnerZip.AddRange(innerZip);
    }

    public bool IsEmpty()
    {
        if ((InnerZip == null || InnerZip.Count == 0) && string.IsNullOrEmpty(Value))
            return true;

        return false;
    }

    public XMLZip GetSafeValue(int index)
    {
        if (index >= 0 && index < InnerZip.Count)
        {
            return InnerZip[index];
        }
        return null;
    }

    public XMLZip Find(string item)
    {
        for (int i = 0; i < InnerZip.Count; i++)
        {
            if (InnerZip[i].Item == item)
                return InnerZip[i];
        }
        return null;
    }

    public XMLZip SearchRoot(string root)
    {
        return SearchRoot(root.Split('.'));
    }

    public XMLZip SearchRoot(params string[] root)
    {
        XMLZip rootXML = this;

        for (int i = 0; i < root.Length; i++)
        {
            rootXML = rootXML.Find(root[i]);
            if (rootXML == null) break;
        }

        return rootXML;
    }

    public XMLZip FindOrCreate(string item)
    {
        for (int i = 0; i < InnerZip.Count; i++)
        {
            if (InnerZip[i].Item == item)
                return InnerZip[i];
        }

        XMLZip zip = Zip(item, "");
        Add(zip);

        return zip;
    }

    public void SafeRemoveAt(int index)
    {
        if (index < 0)
            return;

        if (index >= InnerZip.Count)
            return;

        InnerZip.RemoveAt(index);
    }

    public void SafeRemoveWith(string item)
    {
        for (int i = 0; i < InnerZip.Count; i++)
        {
            if (InnerZip[i].Item == item)
            {
                InnerZip.RemoveAt(i);
                return;
            }
        }
    }

#if UNITY_EDITOR
    public override string ToString()
    {
        string root = "";
        ToString(ref root, 0, this);
        return root;
    }

    private void ToString(ref string text, int step, XMLZip currentRoot)
    {
        string offset = "";
        for (int i = 0; i < step; i++)
            offset += ">> ";

        offset += currentRoot.Item + " " + currentRoot.Value;
        text += offset + "\n";

        if (currentRoot.InnerZip != null && currentRoot.InnerZip.Count > 0)
        {
            for (int i = 0; i < currentRoot.InnerZip.Count; i++)
            {
                ToString(ref text, step + 1, currentRoot.InnerZip[i]);
            }
        }
    }
#endif
}