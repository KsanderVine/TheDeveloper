using UnityEngine;
using System.Xml;
using System.IO;

public static class XMLConstructor
{
    public static string SerializeToXML(XMLZip main)
    {
        XmlDocument doc = new XmlDocument();

        XmlNode node = doc.CreateElement(main.Item);
        doc.AppendChild(node);

        if (main.InnerZip.Count == 0)
        {
            XmlAttribute value = doc.CreateAttribute("value");
            value.Value = main.Value;

            node.Attributes.Append(value);
        }
        else
        {
            for (int i = 0; i < main.InnerZip.Count; i++)
            {
                AddData(doc, node, main.InnerZip[i]);
            }
        }

        string serializedData = doc.InnerXml;
        return serializedData;
    }

    public static XMLZip DeserializeFromXML(string xml)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        XMLZip unzipData = XMLZip.Zip("TEMP", "");

        for (int i = 0; i < xmlDoc.ChildNodes.Count; i++)
        {
            XmlNode currentNode = xmlDoc.ChildNodes[i];

            string zipName = currentNode.Name;
            XMLZip currentZip = new XMLZip(zipName);

            if (currentNode.ChildNodes.Count == 0)
            {
                unzipData.SetValue(currentNode.InnerText.Trim());
            }
            else
            {
                ExtractNode(currentNode, currentZip);
                unzipData.InnerZip.Add(currentZip);
            }
        }

        unzipData = unzipData.InnerZip[0];
        return unzipData;
    }

    public static bool SaveData(XMLZip main, string path, string fileName, bool overrideData)
    {
        if (!overrideData && File.Exists(path))
        {
            return false;
        }

        XmlDocument doc = new XmlDocument();

        XmlNode node = doc.CreateElement(main.Item);
        doc.AppendChild(node);

        if (main.InnerZip.Count == 0)
        {
            XmlAttribute value = doc.CreateAttribute("value");
            value.Value = main.Value;

            node.Attributes.Append(value);
        }
        else
        {
            for (int i = 0; i < main.InnerZip.Count; i++)
            {
                AddData(doc, node, main.InnerZip[i]);
            }
        }

        try
        {
            Debug.Log($"[{nameof(XMLConstructor)}] File saved. Saving path is: " + path);
            doc.Save(path);
        }
        catch (System.InvalidCastException exeption)
        {
            Debug.LogError($"[{nameof(XMLConstructor)}] Attempt to SAVE file with path: {path} - failed:\n{exeption.Message}");
            return false;
        }

        return true;
    }

    private static void AddData(XmlDocument doc, XmlNode node, XMLZip currentZip)
    {
        if (currentZip.InnerZip.Count == 0)
        {
            XmlNode subNode = doc.CreateElement(currentZip.Item);

            string value = currentZip.Value;
            if (value != "" && value.Split('\n').Length > 1)
            {
                value = value.Trim();
                value = "\n" + value + "\n";
            }

            XmlAttribute attribute = doc.CreateAttribute("value");
            attribute.Value = currentZip.Value;

            subNode.Attributes.Append(attribute);
            node.AppendChild(subNode);
        }
        else
        {
            XmlNode subNode = doc.CreateElement(currentZip.Item);
            for (int i = 0; i < currentZip.InnerZip.Count; i++)
            {
                AddData(doc, subNode, currentZip.InnerZip[i]);
            }
            node.AppendChild(subNode);
        }
    }

    public static XMLZip ExtractDocument(XmlDocument xmlDoc)
    {
        XMLZip unzipData = XMLZip.Zip("TEMP", "");

        for (int i = 0; i < xmlDoc.ChildNodes.Count; i++)
        {
            XmlNode currentNode = xmlDoc.ChildNodes[i];

            string zipName = currentNode.Name;
            XMLZip currentZip = new XMLZip(zipName);

            if (currentNode.ChildNodes.Count == 0)
            {
                unzipData.SetValue(currentNode.InnerText.Trim());
            }
            else
            {
                ExtractNode(currentNode, currentZip);
                unzipData.InnerZip.Add(currentZip);
            }
        }

        unzipData = unzipData.InnerZip[0];
        return unzipData;
    }
    
    public static bool TryExtractFile(string path, out XMLZip xmlZip)
    {
        if (File.Exists(path) == false)
        {
            xmlZip = null;
            return false;
        }

        xmlZip = XMLZip.Zip("TEMP", "");
        XmlTextReader textReader = new XmlTextReader(path);

        textReader.Read();
        bool isSuccess = false;

        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(textReader);

            for (int i = 0; i < xmlDoc.ChildNodes.Count; i++)
            {
                XmlNode currentNode = xmlDoc.ChildNodes[i];

                string zipName = currentNode.Name;
                XMLZip currentZip = new XMLZip(zipName);

                if (currentNode.ChildNodes.Count == 0)
                {
                    xmlZip.SetValue(currentNode.InnerText.Trim());
                }
                else
                {
                    ExtractNode(currentNode, currentZip);
                    xmlZip.InnerZip.Add(currentZip);
                }
            }

            xmlZip = xmlZip.InnerZip[0];
            isSuccess = true;
        }
        catch (System.InvalidCastException exeption)
        {
            Debug.LogError($"[{nameof(XMLConstructor)}] Attempt to READ file with path: {path} - failed:\n{exeption.Message}");
        }

        textReader.Close();
        return isSuccess;
    }

    private static void ExtractNode(XmlNode xmlNode, XMLZip parentZip)
    {
        for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
        {
            XmlNode currentNode = xmlNode.ChildNodes[i];

            string zipName = currentNode.Name;
            XMLZip currentZip = new XMLZip(zipName);

            if (currentNode.NodeType == XmlNodeType.Element)
            {
                XmlElement element = currentNode as XmlElement;
                string value = element.GetAttribute("value");

                if (!string.IsNullOrEmpty(value))
                {
                    currentZip.SetValue(value);
                    parentZip.InnerZip.Add(currentZip);
                    continue;
                }
            }

            if (currentNode.ChildNodes.Count == 0)
            {
                parentZip.SetValue(currentNode.InnerText);
            }
            else
            {
                ExtractNode(currentNode, currentZip);
                parentZip.InnerZip.Add(currentZip);
            }
        }
    }

    public static XMLZip FindInnerZip(XMLZip parentZip, string item)
    {
        if (parentZip == null)
        {
            return null;
        }

        if (parentZip.Item == item)
        {
            return parentZip;
        }

        for (int i = 0; i < parentZip.InnerZip.Count; i++)
        {
            XMLZip currentZip = parentZip.InnerZip[i];
            XMLZip child = FindInnerZip(currentZip, item);

            if (child != null && child.Item == item)
            {
                return child;
            }
        }
        return null;
    }
}