using System;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;


namespace CustomToolbar.Editor.ToolbarExtension.Core
{
    public class GitPackageManager : MonoBehaviour
    {
	    private const string PackagesPath = "Packages/manifest.json";

        /// <summary>
        /// Install a Git package by adding it to manifest.json
        /// </summary>
        public static void InstallPackage(string packageId, string gitUrl, string displayName)
        {
            try
            {
                if (IsPackageInstalled(packageId))
                {
                    EditorUtility.DisplayDialog(
                        "Package Already Installed",
                        $"{displayName} is already installed in this project.",
                        "OK"
                    );
                    return;
                }

                // Add package using Package Manager API
                var request = Client.Add(gitUrl);
                
                EditorApplication.update += CheckInstallProgress;

                void CheckInstallProgress()
                {
                    if (request.IsCompleted)
                    {
                        EditorApplication.update -= CheckInstallProgress;

                        if (request.Status == StatusCode.Success)
                        {
                            Debug.Log($"<color=green>Successfully installed {displayName}</color>");
                            EditorUtility.DisplayDialog(
                                "Package Installed",
                                $"{displayName} has been successfully installed!",
                                "OK"
                            );
                        }
                        else if (request.Status >= StatusCode.Failure)
                        {
                            Debug.LogError($"Failed to install {displayName}: {request.Error.message}");
                            EditorUtility.DisplayDialog(
                                "Installation Failed",
                                $"Failed to install {displayName}.\n\nError: {request.Error.message}",
                                "OK"
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error installing {displayName}: {ex.Message}");
                EditorUtility.DisplayDialog(
                    "Installation Error",
                    $"An error occurred while installing {displayName}.\n\n{ex.Message}",
                    "OK"
                );
            }
        }

        /// <summary>
        /// Check if a package is already installed
        /// </summary>
        public static bool IsPackageInstalled(string packageId)
        {
            var listRequest = Client.List(true);
            
            while (!listRequest.IsCompleted)
            {
                // Wait for request to complete
            }

            if (listRequest.Status == StatusCode.Success)
            {
                foreach (var package in listRequest.Result)
                {
                    if (package.name == packageId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Remove a package
        /// </summary>
        public static void RemovePackage(string packageId, string displayName)
        {
            if (!IsPackageInstalled(packageId))
            {
                EditorUtility.DisplayDialog(
                    "Package Not Found",
                    $"{displayName} is not installed in this project.",
                    "OK"
                );
                return;
            }

            bool confirm = EditorUtility.DisplayDialog(
                "Remove Package",
                $"Are you sure you want to remove {displayName}?",
                "Yes",
                "No"
            );

            if (!confirm)
                return;

            var request = Client.Remove(packageId);

            EditorApplication.update += CheckRemoveProgress;

            void CheckRemoveProgress()
            {
                if (request.IsCompleted)
                {
                    EditorApplication.update -= CheckRemoveProgress;

                    if (request.Status == StatusCode.Success)
                    {
                        Debug.Log($"<color=yellow>Successfully removed {displayName}</color>");
                        EditorUtility.DisplayDialog(
                            "Package Removed",
                            $"{displayName} has been removed.",
                            "OK"
                        );
                    }
                    else if (request.Status >= StatusCode.Failure)
                    {
                        Debug.LogError($"Failed to remove {displayName}: {request.Error.message}");
                    }
                }
            }
        }

        /// <summary>
        /// Get package version if installed
        /// </summary>
        public static string GetPackageVersion(string packageId)
        {
            var listRequest = Client.List(true);
            
            while (!listRequest.IsCompleted)
            {
                // Wait for request to complete
            }

            if (listRequest.Status == StatusCode.Success)
            {
                foreach (var package in listRequest.Result)
                {
                    if (package.name == packageId)
                    {
                        return package.version;
                    }
                }
            }

            return null;
        }

    }
}
