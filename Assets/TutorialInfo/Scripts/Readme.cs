using System;
using UnityEngine;

/**
 * @brief ScriptableObject for displaying readme information in the Unity Editor
 * 
 * This class provides a structured way to create and display tutorial or
 * documentation information within the Unity Editor interface
 */
public class Readme : ScriptableObject
{
    public Texture2D icon;     // The icon to display with the readme
    public string title;       // The title of the readme
    public Section[] sections; // The content sections of the readme
    public bool loadedLayout;  // Whether the editor layout has been loaded for this readme

    /**
     * @brief Represents a section of content in the readme
     * 
     * Each section can contain a heading, text content, and an optional link
     */
    [Serializable]
    public class Section
    {
        public string heading;  // Section heading
        public string text;     // Main content text
        public string linkText; // Text for the hyperlink
        public string url;      // URL for the hyperlink
    }
}
