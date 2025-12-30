using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace CustomToolbar.Editor.ToolbarExtension.Core
{
    public static class CustomToolbarManager
    {
	    // --------------------------------------- Settings Button -----------------------------------------------------
	    [MainToolbarElement("CustomTools/Settings")]
	    public static MainToolbarElement CreateSettingsButton()
	    {
		    var icon = EditorGUIUtility.IconContent("SettingsIcon").image as Texture2D;
		    var content = new MainToolbarContent(icon, "Open Project Settings");
		    
		    
		    
		    return new MainToolbarButton( content,() => { SettingsService.OpenProjectSettings(); });
	    }

  	    // ------------------------------------- Git Package Buttons ---------------------------------------------------
        [MainToolbarElement("CustomTools/Git/TriInspector")]
        public static MainToolbarElement CreateTriInspectorButton()
        {
	        var icon = EditorGUIUtility.IconContent("Package Manager").image as Texture2D;
	        var content = new MainToolbarContent(icon, "Install Tri-Inspector Package");
	        content.text = "Tri";
	        
	        var button = new MainToolbarButton(content, () =>
	        {
		        GitPackageManager.InstallPackage(
			        "com.codewriter.tri-inspector",
			        "https://github.com/codewriter-packages/Tri-Inspector.git",
			        "Tri-Inspector"
		        );
	        });

	        //button.content.text = "Tri";
	        return button;
        }


    	// ---------------------------------------- Public Properties --------------------------------------------------


    	// ---------------------------------------- Private Properties -------------------------------------------------


    	// ------------------------------------------ Helper Method ----------------------------------------------------

    }
}