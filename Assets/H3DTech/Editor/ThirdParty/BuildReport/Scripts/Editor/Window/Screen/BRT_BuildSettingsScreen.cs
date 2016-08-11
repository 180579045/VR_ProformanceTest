using UnityEngine;
using UnityEditor;
using System.Collections;



namespace BuildReportTool.Window.Screen
{

public class BuildSettings : BaseScreen
{
	public override string Name { get{ return Labels.BUILD_SETTINGS_CATEGORY_LABEL; } }

	public override void RefreshData(BuildInfo buildReport)
	{
		_selectedSettingsIdxFromDropdownBox = UnityBuildSettingsUtility.GetIdxFromBuildReportValues(buildReport);
	}

	Vector2 _scrollPos;

	const int SETTING_SPACING = 4;
	const int SETTINGS_GROUP_TITLE_SPACING = 3;
	const int SETTINGS_GROUP_SPACING = 18;
	const int SETTINGS_GROUP_MINOR_SPACING = 12;


	void DrawSetting(string name, bool val, bool showEvenIfEmpty = false)
	{
		DrawSetting(name, val.ToString(), showEvenIfEmpty);
	}

	void DrawSetting(string name, int val, bool showEvenIfEmpty = false)
	{
		DrawSetting(name, val.ToString(), showEvenIfEmpty);
	}

	void DrawSetting(string name, uint val, bool showEvenIfEmpty = false)
	{
		DrawSetting(name, val.ToString(), showEvenIfEmpty);
	}

	void DrawSetting(string name, string val, bool showEvenIfEmpty = false)
	{
		if (string.IsNullOrEmpty(val) && !showEvenIfEmpty)
		{
			return;
		}

		GUILayout.BeginHorizontal();
			GUILayout.Label(name, Settings.SETTING_NAME_STYLE_NAME);
			GUILayout.Space(2);
			GUILayout.TextField(val, Settings.SETTING_VALUE_STYLE_NAME);
		GUILayout.EndHorizontal();
		GUILayout.Space(SETTING_SPACING);
	}

	void DrawSetting(string name, string[] val, bool showEvenIfEmpty = false)
	{
		if ((val == null || val.Length == 0) && !showEvenIfEmpty)
		{
			return;
		}

		GUILayout.BeginHorizontal();
			GUILayout.Label(name, Settings.SETTING_NAME_STYLE_NAME);
			GUILayout.Space(2);


			GUILayout.BeginVertical();
			for (int n = 0, len = val.Length; n < len; ++n)
			{
				GUILayout.TextField(val[n], Settings.SETTING_VALUE_STYLE_NAME);
			}
			GUILayout.EndVertical();

		GUILayout.EndHorizontal();
		GUILayout.Space(SETTING_SPACING);
	}

	void DrawSettingsGroupTitle(string name)
	{
		GUILayout.Label(name, Settings.INFO_TITLE_STYLE_NAME);
		GUILayout.Space(SETTINGS_GROUP_TITLE_SPACING);
	}

	// =================================================================================

	BuildSettingCategory _settingsShown = BuildSettingCategory.None;

	// ----------------------------------------------

	bool IsShowingWebPlayerSettings
	{
		get
		{
			return _settingsShown == BuildSettingCategory.WebPlayer;
		}
	}

	// ----------------------------------------------

	bool IsShowingStandaloneSettings
	{
		get
		{
			return IsShowingWindowsDesktopSettings || IsShowingMacSettings || IsShowingLinuxSettings;
		}
	}

	bool IsShowingWindowsDesktopSettings
	{
		get
		{
			return _settingsShown == BuildSettingCategory.WindowsDesktopStandalone;
		}
	}

	bool IsShowingMacSettings
	{
		get
		{
			return _settingsShown == BuildSettingCategory.MacStandalone;
		}
	}

	bool IsShowingLinuxSettings
	{
		get
		{
			return _settingsShown == BuildSettingCategory.LinuxStandalone;
		}
	}

	// ----------------------------------------------

	bool IsShowingMobileSettings
	{
		get
		{
			return IsShowingiOSSettings || IsShowingAndroidSettings || IsShowingBlackberrySettings;
		}
	}

	bool IsShowingiOSSettings
	{
		get
		{
			return _settingsShown == BuildSettingCategory.iOS;
		}
	}

	bool IsShowingAndroidSettings
	{
		get
		{
			return _settingsShown == BuildSettingCategory.Android;
		}
	}

	bool IsShowingBlackberrySettings
	{
		get
		{
			return _settingsShown == BuildSettingCategory.Blackberry;
		}
	}

	// ----------------------------------------------

	bool IsShowingXbox360Settings
	{
		get
		{
			return _settingsShown == BuildSettingCategory.Xbox360;
		}
	}

	bool IsShowingPS3Settings
	{
		get
		{
			return _settingsShown == BuildSettingCategory.PS3;
		}
	}

	bool IsShowingPSVitaSettings
	{
		get
		{
			return _settingsShown == BuildSettingCategory.PSVita;
		}
	}

	// =================================================================================

	int _selectedSettingsIdxFromDropdownBox = 0;

	GUIContent[] _settingDropdownBoxLabels;
	string _buildTargetOfReport = string.Empty;

	void InitializeDropdownBoxLabelsIfNeeded()
	{
		if (_settingDropdownBoxLabels != null) { return; }

		_settingDropdownBoxLabels = UnityBuildSettingsUtility.GetBuildSettingsCategoryListForDropdownBox();
	}



	// =================================================================================

	public override void DrawGUI(Rect position, BuildInfo buildReportToDisplay)
	{
		BuildSettingCategory b = ReportGenerator.GetBuildSettingCategoryFromBuildValues(buildReportToDisplay);
		_buildTargetOfReport = UnityBuildSettingsUtility.GetReadableBuildSettingCategory(b);

		UnityBuildSettings settings = buildReportToDisplay.UnityBuildSettings;

		if (settings == null)
		{
			Utility.DrawCentralMessage(position, "No \"Project Settings\" recorded in this build report.");
			return;
		}

		// ----------------------------------------------------------
		// top bar

		GUILayout.Space(1);
		GUILayout.BeginHorizontal();

			GUILayout.Label(" ", Settings.TOP_BAR_BG_STYLE_NAME);

			GUILayout.Space(8);
			GUILayout.Label("Build Target: ", Settings.TOP_BAR_LABEL_STYLE_NAME);

			InitializeDropdownBoxLabelsIfNeeded();
			_selectedSettingsIdxFromDropdownBox = EditorGUILayout.Popup(_selectedSettingsIdxFromDropdownBox, _settingDropdownBoxLabels, Settings.FILE_FILTER_POPUP_STYLE_NAME);
			GUILayout.Space(15);

			GUILayout.Label("Note: Project was built in " + _buildTargetOfReport + " target", Settings.TOP_BAR_LABEL_STYLE_NAME);

			GUILayout.FlexibleSpace();

			_settingsShown = UnityBuildSettingsUtility.GetSettingsCategoryFromIdx(_selectedSettingsIdxFromDropdownBox);

		GUILayout.EndHorizontal();

		// ----------------------------------------------------------

		_scrollPos = GUILayout.BeginScrollView(_scrollPos);

		GUILayout.BeginHorizontal();

		GUILayout.Space(10);
		GUILayout.BeginVertical();


		GUILayout.Space(10);



		// =================================================================
		DrawSettingsGroupTitle("Project");

		DrawSetting("Product name:", settings.ProductName);
		DrawSetting("Company name:", settings.CompanyName);
		DrawSetting("Build type:", buildReportToDisplay.BuildType);
		DrawSetting("Unity version:", buildReportToDisplay.UnityVersion);
		DrawSetting("Using Pro license:", settings.UsingAdvancedLicense);

		if (IsShowingiOSSettings)
		{
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
			DrawSetting("App display name:", settings.iOSAppDisplayName);
			DrawSetting("Bundle Identifier:", settings.MobileBundleIdentifier);
			DrawSetting("Bundle Version:", settings.MobileBundleVersion);
		}
		else if (IsShowingAndroidSettings)
		{
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
			DrawSetting("Package Identifier:", settings.MobileBundleIdentifier);
			DrawSetting("Version Name:", settings.MobileBundleVersion);
			DrawSetting("Version code:", settings.AndroidVersionCode);
		}
		else if (IsShowingXbox360Settings)
		{
			DrawSetting("Xbox 360 Title ID:", settings.Xbox360TitleId, true);
		}

		GUILayout.Space(SETTINGS_GROUP_SPACING);



		// =================================================================
		DrawSettingsGroupTitle("Paths");

		DrawSetting("Unity path:", buildReportToDisplay.EditorAppContentsPath);
		DrawSetting("Project path:", buildReportToDisplay.ProjectAssetsPath);
		DrawSetting("Build path:", buildReportToDisplay.BuildFilePath);

		GUILayout.Space(SETTINGS_GROUP_SPACING);




		// =================================================================
		DrawSettingsGroupTitle("Build Settings");

		// --------------------------------------------------
		// build settings
		if (IsShowingWebPlayerSettings)
		{
			DrawSetting("Web player streaming:", settings.WebPlayerEnableStreaming);
			DrawSetting("Web player offline deployment:", settings.WebPlayerDeployOffline);
			DrawSetting("First streamed level with \"Resources\" assets:", settings.WebPlayerFirstStreamedLevelWithResources);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}
		else if (IsShowingiOSSettings)
		{
			DrawSetting("SDK version:", settings.iOSSDKVersionUsed);
			DrawSetting("Target iOS version:", settings.iOSTargetOSVersion);
			DrawSetting("Target device:", settings.iOSTargetDevice);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}
		else if (IsShowingAndroidSettings)
		{
			DrawSetting("Build subtarget:", settings.AndroidBuildSubtarget);
			DrawSetting("Min SDK version:", settings.AndroidMinSDKVersion);
			DrawSetting("Target device:", settings.AndroidTargetDevice);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);


			DrawSetting("Force Internet permission:", settings.AndroidForceInternetPermission);
			DrawSetting("Force SD card permission:", settings.AndroidForceSDCardPermission);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);


			DrawSetting("Key alias name:", settings.AndroidKeyAliasName);
			DrawSetting("Keystore name:", settings.AndroidKeystoreName);
		}
		else if (IsShowingBlackberrySettings)
		{
			DrawSetting("Build subtarget:", settings.BlackBerryBuildSubtarget);
			DrawSetting("Build type:", settings.BlackBerryBuildType);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)

#else
			DrawSetting("Author ID:", settings.BlackBerryAuthorID);
#endif

			DrawSetting("Device address:", settings.BlackBerryDeviceAddress);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

			DrawSetting("Save log path:", settings.BlackBerrySaveLogPath);
			DrawSetting("Token path:", settings.BlackBerryTokenPath);

			DrawSetting("Token author:", settings.BlackBerryTokenAuthor);
			DrawSetting("Token expiration:", settings.BlackBerryTokenExpiration);
		}
		else if (IsShowingXbox360Settings)
		{
			DrawSetting("Build subtarget:", settings.Xbox360BuildSubtarget);
			DrawSetting("Run method:", settings.Xbox360RunMethod);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
			DrawSetting("Image .xex filepath:", settings.Xbox360ImageXexFilePath, true);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
			DrawSetting(".spa filepath:", settings.Xbox360SpaFilePath, true);
			DrawSetting("Auto-generate .spa:", settings.Xbox360AutoGenerateSpa);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}
		else if (IsShowingPS3Settings)
		{
			DrawSetting("Build subtarget:", settings.SCEBuildSubtarget);

			DrawSetting("NP Communications ID:", settings.PS3NpCommsId);
			DrawSetting("NP Communications Signature:", settings.PS3NpCommsSig);

			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

			DrawSetting("Title config filepath:", settings.PS3TitleConfigFilePath, true);
			DrawSetting("DLC config filepath:", settings.PS3DLCConfigFilePath, true);
			DrawSetting("Thumbnail filepath:", settings.PS3ThumbnailFilePath, true);
			DrawSetting("Background image filepath:", settings.PS3BackgroundImageFilePath, true);
			DrawSetting("Background sound filepath:", settings.PS3BackgroundSoundFilePath, true);
			DrawSetting("Trophy package path:", settings.PS3TrophyPackagePath, true);

			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);

			DrawSetting("Video memory for vertex buffers:", settings.PS3VideoMemoryForVertexBuffers);
			DrawSetting("Boot check max save game size (KB):", settings.PS3BootCheckMaxSaveGameSizeKB);
			DrawSetting("Save game slots:", settings.PS3SaveGameSlots);


			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}
		else if (IsShowingPSVitaSettings)
		{
			DrawSetting("Trophy package path:", settings.PSVTrophyPackagePath);
			DrawSetting("Params Sfx path:", settings.PSVParamSfxPath);

			DrawSetting("NP Communications ID:", settings.PSVNpCommsId);
			DrawSetting("NP Communications Signature:", settings.PSVNpCommsSig);
		}
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)
			
			
#else
		if (IsShowingiOSSettings)
		{
			DrawSetting("Is appended build:", settings.iOSAppendedToProject);
		}
#endif

		DrawSetting("Install in build folder:", settings.InstallInBuildFolder);



		GUILayout.Space(SETTINGS_GROUP_SPACING);



		// =================================================================
		DrawSettingsGroupTitle("Runtime Settings");

		if (IsShowingiOSSettings)
		{
			DrawSetting("Hide status bar:", settings.MobileHideStatusBar);
			DrawSetting("Status bar style:", settings.iOSStatusBarStyle);
			DrawSetting("Accelerometer frequency:", settings.MobileAccelerometerFrequency);
			DrawSetting("Requires persistent Wi-Fi:", settings.iOSRequiresPersistentWiFi);
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)		
#else
			DrawSetting("Exit on suspend:", settings.iOSExitOnSuspend);
#endif
			DrawSetting("Activity indicator on loading:", settings.iOSShowProgressBarInLoadingScreen);

			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}
		else if (IsShowingAndroidSettings)
		{
			DrawSetting("Hide status bar:", settings.MobileHideStatusBar);
			DrawSetting("Accelerometer frequency:", settings.MobileAccelerometerFrequency);
			DrawSetting("Activity indicator on loading:", settings.AndroidShowProgressBarInLoadingScreen);
			DrawSetting("Splash screen scale:", settings.AndroidSplashScreenScaleMode);

			DrawSetting("Preferred install location:", settings.AndroidPreferredInstallLocation);

			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}
		else if (!IsShowingiOSSettings && !IsShowingAndroidSettings && IsShowingMobileSettings) // any mobile except iOS, Android
		{
			DrawSetting("Hide status bar:", settings.MobileHideStatusBar);
			DrawSetting("Accelerometer frequency:", settings.MobileAccelerometerFrequency);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}
		else if (IsShowingXbox360Settings)
		{
			DrawSetting("Enable avatar:", settings.Xbox360EnableAvatar);
			DrawSetting("Enable Kinect:", settings.Xbox360EnableKinect);
			DrawSetting("Deploy Kinect resources:", settings.Xbox360DeployKinectResources);
			DrawSetting("Enable Kinect auto-tracking:", settings.Xbox360EnableKinectAutoTracking);
			DrawSetting("Enable speech:", settings.Xbox360EnableSpeech);
			DrawSetting("Speech DB:", settings.Xbox360SpeechDB);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}
		else if (IsShowingBlackberrySettings)
		{
			DrawSetting("Has camera permissions:", settings.BlackBerryHasCamPermissions);
			DrawSetting("Has microphone permissions:", settings.BlackBerryHasMicPermissions);
			DrawSetting("Has GPS permissions:", settings.BlackBerryHasGpsPermissions);
			DrawSetting("Has ID permissions:", settings.BlackBerryHasIdPermissions);
			DrawSetting("Has shared permissions:", settings.BlackBerryHasSharedPermissions);
		}

		if (IsShowingStandaloneSettings || IsShowingWebPlayerSettings || IsShowingBlackberrySettings)
		{
			DrawSetting("Run in background:", settings.RunInBackground);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}


		// --------------------------------------------------
		// security settings
		if (IsShowingMacSettings)
		{
			DrawSetting("Use App Store validation:", settings.MacUseAppStoreValidation);
		}
		else if (IsShowingAndroidSettings)
		{
			DrawSetting("Use license verification:", settings.AndroidUseLicenseVerification);
		}


		GUILayout.Space(SETTINGS_GROUP_SPACING);



		// =================================================================
		DrawSettingsGroupTitle("Debug Settings");

		DrawSetting("Debug Log enabled:", settings.EnableDebugLog);
		DrawSetting("Is development build:", settings.EnableDevelopmentBuild);
		DrawSetting("Auto-connect to Profiler:", settings.ConnectProfiler);
		DrawSetting("Allow debugger:", settings.EnableSourceDebugging);


		GUILayout.Space(SETTINGS_GROUP_SPACING);



		// =================================================================
		DrawSettingsGroupTitle("Code Settings");

		if (IsShowingiOSSettings)
		{
			DrawSetting("Symlink libraries:", settings.iOSSymlinkLibraries);
			DrawSetting("Script call optimized:", settings.iOSScriptCallOptimizationUsed);
			//GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}

		DrawSetting("Stripping level:", settings.StrippingLevelUsed);
		DrawSetting(".NET API compatibility level:", settings.NETApiCompatibilityLevel);
		DrawSetting("Explicit null checks:", settings.EnableExplicitNullChecks);
		DrawSetting("AOT options:", settings.AOTOptions, true);
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)
			
#else
		DrawSetting("Physics code stripped:", settings.StripPhysicsCode);
#endif

		DrawSetting("Script Compilation Defines:", settings.CompileDefines);

		GUILayout.Space(SETTINGS_GROUP_SPACING);



		// =================================================================
		DrawSettingsGroupTitle("Graphics Settings");

		if (IsShowingMobileSettings)
		{
			DrawSetting("Default interface orientation:", settings.MobileDefaultOrientationUsed);

			DrawSetting("Use OS screen auto-rotate:", settings.MobileEnableOSAutorotation);
			DrawSetting("Auto-rotate to portrait:", settings.MobileEnableAutorotateToPortrait);
			DrawSetting("Auto-rotate to reverse portrait:", settings.MobileEnableAutorotateToReversePortrait);
			DrawSetting("Auto-rotate to landscape left:", settings.MobileEnableAutorotateToLandscapeLeft);
			DrawSetting("Auto-rotate to landscape right:", settings.MobileEnableAutorotateToLandscapeRight);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}
		else if (IsShowingStandaloneSettings)
		{
			string standaloneScreenSize = settings.StandaloneDefaultScreenWidth + " x " + settings.StandaloneDefaultScreenHeight;
			DrawSetting("Default screen size:", standaloneScreenSize);
			DrawSetting("Resolution dialog:", settings.StandaloneResolutionDialogSettingUsed);
			DrawSetting("Full screen by default:", settings.StandaloneFullScreenByDefault);
			DrawSetting("Capture single screen:", settings.StandaloneCaptureSingleScreen);
			DrawSetting("Force single instance:", settings.StandaloneForceSingleInstance);
			DrawSetting("Resizable window:", settings.StandaloneEnableResizableWindow);
			DrawSetting("Use stereoscopic 3d:", settings.StandaloneUseStereoscopic3d);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}

		if (IsShowingWebPlayerSettings)
		{
			string webScreenSize = settings.WebPlayerDefaultScreenWidth + " x " + settings.WebPlayerDefaultScreenHeight;
			DrawSetting("Screen size:", webScreenSize);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}
		else if (IsShowingWindowsDesktopSettings)
		{
			DrawSetting("Use Direct3D 11 if available:", settings.WinUseDirect3D11IfAvailable);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}
		else if (IsShowingMacSettings)
		{
			DrawSetting("Fullscreen mode:", settings.MacFullscreenModeUsed);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}
		else if (IsShowingiOSSettings)
		{
			DrawSetting("Target resolution:", settings.iOSTargetResolution);
			DrawSetting("App icon pre-rendered:", settings.iOSIsIconPrerendered);
			GUILayout.Space(SETTINGS_GROUP_MINOR_SPACING);
		}
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)		
#else
		else if (IsShowingAndroidSettings)
		{
			DrawSetting("Use 24-bit depth buffer:", settings.AndroidUse24BitDepthBuffer);
		}
#endif

		DrawSetting("Use 32-bit display buffer:", settings.Use32BitDisplayBuffer);
		DrawSetting("Color space:", settings.ColorSpaceUsed);
		DrawSetting("Strip unused mesh components:", settings.StripUnusedMeshComponents);
		DrawSetting("Use multi-threaded rendering:", settings.UseMultithreadedRendering);
		DrawSetting("Rendering path:", settings.RenderingPathUsed);

		GUILayout.Space(SETTINGS_GROUP_SPACING);








		GUILayout.Space(10);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.EndScrollView();
	}
}

}



public static class ScriptReference
{

	//
	//   Example usage:
	//
	//   if (GUILayout.Button("doc"))
	//   {
	//      ScriptReference.GoTo("EditorUserBuildSettings.development");
	//   }
	//
	public static void GoTo(string pageName)
	{
		string pageUrl = "file:///";

		pageUrl += EditorApplication.applicationContentsPath;

		// unity 3
		pageUrl += "/Documentation/Documentation/ScriptReference/";

		pageUrl += pageName.Replace(".", "-");
		pageUrl += ".html";

		Debug.Log("going to: " + pageUrl);

		Application.OpenURL(pageUrl);
	}
}
