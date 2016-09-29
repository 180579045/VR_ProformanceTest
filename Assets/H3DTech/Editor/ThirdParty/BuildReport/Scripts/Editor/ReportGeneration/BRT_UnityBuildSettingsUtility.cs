using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



namespace BuildReportTool
{
public static class UnityBuildSettingsUtility
{
	// ================================================================================================

	public static GUIContent[] GetBuildSettingsCategoryListForDropdownBox()
	{
		// WARNING! changing contents here will require changing code in:
		//
		//  SetSelectedSettingsIdxFromBuildReportValues
		//  SetSettingsShownFromIdx
		//
		// as they rely on the array indices
		//
		return new GUIContent[]
		{
			/* 0 */ new GUIContent("Windows"),
			/* 1 */ new GUIContent("Mac"),
			/* 2 */ new GUIContent("Linux"),

			/* 3 */ new GUIContent("Web"),

			/* 4 */ new GUIContent("iOS"),
			/* 5 */ new GUIContent("Android"),
			/* 6 */ new GUIContent("Blackberry"),

			/* 7 */ new GUIContent("Xbox 360"),
			/* 8 */ new GUIContent("Playstation 3"),

			/* 9 */ new GUIContent("Playstation Vita (Native)"),
		};
	}


	public static int GetIdxFromBuildReportValues(BuildInfo buildReportToDisplay)
	{
		BuildSettingCategory b = ReportGenerator.GetBuildSettingCategoryFromBuildValues(buildReportToDisplay);

		switch (b)
		{
			case BuildSettingCategory.WindowsDesktopStandalone:
				return 0;
			case BuildSettingCategory.MacStandalone:
				return 1;
			case BuildSettingCategory.LinuxStandalone:
				return 2;

			case BuildSettingCategory.WebPlayer:
				return 3;

			case BuildSettingCategory.iOS:
				return 4;
			case BuildSettingCategory.Android:
				return 5;
			case BuildSettingCategory.Blackberry:
				return 6;

			case BuildSettingCategory.Xbox360:
				return 7;
			case BuildSettingCategory.PS3:
				return 8;

			case BuildSettingCategory.PSVita:
				return 9;
		}
		return -1;
	}

	public static BuildSettingCategory GetSettingsCategoryFromIdx(int idx)
	{
		switch (idx)
		{
			case 0:
				return BuildSettingCategory.WindowsDesktopStandalone;
			case 1:
				return BuildSettingCategory.MacStandalone;
			case 2:
				return BuildSettingCategory.LinuxStandalone;
			case 3:
				return BuildSettingCategory.WebPlayer;
			case 4:
				return BuildSettingCategory.iOS;
			case 5:
				return BuildSettingCategory.Android;
			case 6:
				return BuildSettingCategory.Blackberry;
			case 7:
				return BuildSettingCategory.Xbox360;
			case 8:
				return BuildSettingCategory.PS3;
			case 9:
				return BuildSettingCategory.PSVita;
		}

		return BuildSettingCategory.None;
	}

	public static string GetReadableBuildSettingCategory(BuildSettingCategory b)
	{
		switch (b)
		{
			case BuildSettingCategory.WindowsDesktopStandalone:
				return "Windows";

			case BuildSettingCategory.WindowsPhone8:
				return "Windows Phone 8";

			case BuildSettingCategory.MacStandalone:
				return "Mac";

			case BuildSettingCategory.LinuxStandalone:
				return "Linux";


			case BuildSettingCategory.WebPlayer:
				return "Web";


			case BuildSettingCategory.Xbox360:
				return "Xbox 360";
			case BuildSettingCategory.XboxOne:
				return "Xbox One";

			case BuildSettingCategory.PS3:
				return "Playstation 3";
			case BuildSettingCategory.PS4:
				return "Playstation 4";

			case BuildSettingCategory.PSVita:
				return "Playstation Vita (Native)";

			case BuildSettingCategory.PSM:
				return "Playstation Mobile";

			case BuildSettingCategory.WebGL:
				return "Web GL";
		}

		return b.ToString();
	}



	// ================================================================================================

	public static void Populate(UnityBuildSettings settings)
	{
		PopulateGeneralSettings(settings);
		PopulateWebSettings(settings);
		PopulateStandaloneSettings(settings);
		PopulateMobileSettings(settings);
		PopulateBigConsoleGen07Settings(settings);
	}


	public static void PopulateGeneralSettings(UnityBuildSettings settings)
	{
		settings.CompanyName = PlayerSettings.companyName;
		settings.ProductName = PlayerSettings.productName;

		settings.UsingAdvancedLicense = PlayerSettings.advancedLicense;

		settings.InstallInBuildFolder = EditorUserBuildSettings.installInBuildFolder;



		// debug settings
		// ---------------------------------------------------------------
		settings.EnableDevelopmentBuild = EditorUserBuildSettings.development;

		settings.EnableDebugLog = PlayerSettings.usePlayerLog;

		settings.ConnectProfiler = EditorUserBuildSettings.connectProfiler;

		settings.EnableSourceDebugging = EditorUserBuildSettings.allowDebugging;


		// code settings
		// ---------------------------------------------------------------

		Dictionary<string, DldUtil.GetRspDefines.Entry> customDefines = DldUtil.GetRspDefines.GetDefines();

		List<string> defines = new List<string>();
		defines.AddRange(EditorUserBuildSettings.activeScriptCompilationDefines);
		

		foreach(KeyValuePair<string, DldUtil.GetRspDefines.Entry> customDefine in customDefines)
		{
			if (customDefine.Value.TimesDefinedInBuiltIn == 0)
			{
				defines.Add(customDefine.Key);
			}
		}

		settings.CompileDefines = defines.ToArray();




		settings.StrippingLevelUsed = PlayerSettings.strippingLevel.ToString();

		settings.NETApiCompatibilityLevel = PlayerSettings.apiCompatibilityLevel.ToString();

		settings.AOTOptions = PlayerSettings.aotOptions;

		settings.EnableExplicitNullChecks = EditorUserBuildSettings.explicitNullChecks;
		
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)

		
#else

		settings.StripPhysicsCode = PlayerSettings.stripPhysics;
#endif


		// rendering settings
		// ---------------------------------------------------------------
		settings.ColorSpaceUsed = PlayerSettings.colorSpace.ToString();

		settings.StripUnusedMeshComponents = PlayerSettings.stripUnusedMeshComponents;
		settings.UseMultithreadedRendering = PlayerSettings.MTRendering;

		settings.UseGPUSkinning = PlayerSettings.gpuSkinning;

		settings.RenderingPathUsed = PlayerSettings.renderingPath.ToString();


		// shared settings
		// ---------------------------------------------------------------

		// shared between web and standalone
		settings.RunInBackground = PlayerSettings.runInBackground;
	}

	public static void PopulateWebSettings(UnityBuildSettings settings)
	{
		// web player settings
		// ---------------------------------------------------------------
		settings.WebPlayerDefaultScreenWidth = PlayerSettings.defaultWebScreenWidth;
		settings.WebPlayerDefaultScreenHeight = PlayerSettings.defaultWebScreenHeight;

		settings.WebPlayerEnableStreaming = EditorUserBuildSettings.webPlayerStreamed;
		settings.WebPlayerDeployOffline = EditorUserBuildSettings.webPlayerOfflineDeployment;

		//settings.WebPlayerFirstStreamedLevelWithResources = PlayerSettings.firstStreamedLevelWithResources;

		// flash player settings
		// ---------------------------------------------------------------
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)
					
#else
		settings.FlashBuildSubtarget = EditorUserBuildSettings.flashBuildSubtarget.ToString();
#endif
	}



	public static void PopulateStandaloneSettings(UnityBuildSettings settings)
	{
		// standalone (windows/mac/linux) build settings
		// ---------------------------------------------------------------
		settings.StandaloneResolutionDialogSettingUsed = PlayerSettings.displayResolutionDialog.ToString();

		settings.StandaloneDefaultScreenWidth = PlayerSettings.defaultScreenWidth;
		settings.StandaloneDefaultScreenHeight = PlayerSettings.defaultScreenHeight;

		settings.StandaloneFullScreenByDefault = PlayerSettings.defaultIsFullScreen;

		settings.StandaloneCaptureSingleScreen = PlayerSettings.captureSingleScreen;

		settings.StandaloneForceSingleInstance = PlayerSettings.forceSingleInstance;
		settings.StandaloneEnableResizableWindow = PlayerSettings.resizableWindow;



		// windows only build settings
		// ---------------------------------------------------------------
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)
			
#else
		settings.WinUseDirect3D11IfAvailable = PlayerSettings.useDirect3D11;
#endif


		// mac only build settings
		// ---------------------------------------------------------------
		settings.MacUseAppStoreValidation = PlayerSettings.useMacAppStoreValidation;
		settings.MacFullscreenModeUsed = PlayerSettings.macFullscreenMode.ToString();
	}



	public static void PopulateMobileSettings(UnityBuildSettings settings)
	{
		// Mobile build settings
		// ---------------------------------------------------------------

		settings.MobileBundleIdentifier = PlayerSettings.bundleIdentifier; // ("Bundle Identifier" in iOS, "Package Identifier" in Android)
		settings.MobileBundleVersion = PlayerSettings.bundleVersion; // ("Bundle Version" in iOS, "Version Name" in Android)
		settings.MobileHideStatusBar = PlayerSettings.statusBarHidden;

		settings.MobileAccelerometerFrequency = PlayerSettings.accelerometerFrequency;

		settings.MobileDefaultOrientationUsed = PlayerSettings.defaultInterfaceOrientation.ToString();
		settings.MobileEnableAutorotateToPortrait = PlayerSettings.allowedAutorotateToPortrait;
		settings.MobileEnableAutorotateToReversePortrait = PlayerSettings.allowedAutorotateToPortraitUpsideDown;
		settings.MobileEnableAutorotateToLandscapeLeft = PlayerSettings.allowedAutorotateToLandscapeLeft;
		settings.MobileEnableAutorotateToLandscapeRight = PlayerSettings.allowedAutorotateToLandscapeRight;
		settings.MobileEnableOSAutorotation = PlayerSettings.useAnimatedAutorotation;

		settings.Use32BitDisplayBuffer = PlayerSettings.use32BitDisplayBuffer;



		// iOS only build settings
		// ---------------------------------------------------------------
			
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)				
#else
		settings.iOSAppendedToProject = EditorUserBuildSettings.appendProject;
#endif
		settings.iOSSymlinkLibraries = EditorUserBuildSettings.symlinkLibraries;

		settings.iOSAppDisplayName = PlayerSettings.iOS.applicationDisplayName;

		settings.iOSScriptCallOptimizationUsed = PlayerSettings.iOS.scriptCallOptimization.ToString();

		settings.iOSSDKVersionUsed = PlayerSettings.iOS.sdkVersion.ToString();
		settings.iOSTargetOSVersion = PlayerSettings.iOS.targetOSVersion.ToString();

		settings.iOSTargetDevice = PlayerSettings.iOS.targetDevice.ToString();
		//settings.iOSTargetResolution = PlayerSettings.iOS.targetResolution.ToString();

		settings.iOSIsIconPrerendered = PlayerSettings.iOS.prerenderedIcon;

		settings.iOSRequiresPersistentWiFi = PlayerSettings.iOS.requiresPersistentWiFi.ToString();

		settings.iOSStatusBarStyle = PlayerSettings.iOS.statusBarStyle.ToString();
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)		
#else
		settings.iOSExitOnSuspend = PlayerSettings.iOS.exitOnSuspend;
#endif

		settings.iOSShowProgressBarInLoadingScreen = PlayerSettings.iOS.showActivityIndicatorOnLoading.ToString();


		// Android only build settings
		// ---------------------------------------------------------------

		settings.AndroidBuildSubtarget = EditorUserBuildSettings.androidBuildSubtarget.ToString();

		settings.AndroidUseAPKExpansionFiles = PlayerSettings.Android.useAPKExpansionFiles;


		settings.AndroidUseLicenseVerification = PlayerSettings.Android.licenseVerification;

#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)		
#else
		settings.AndroidUse24BitDepthBuffer = PlayerSettings.Android.use24BitDepthBuffer;
#endif

		settings.AndroidVersionCode = PlayerSettings.Android.bundleVersionCode;

		settings.AndroidMinSDKVersion = PlayerSettings.Android.minSdkVersion.ToString();
		settings.AndroidTargetDevice = PlayerSettings.Android.targetDevice.ToString();

		settings.AndroidSplashScreenScaleMode = PlayerSettings.Android.splashScreenScale.ToString();

		settings.AndroidPreferredInstallLocation = PlayerSettings.Android.preferredInstallLocation.ToString();

		settings.AndroidForceInternetPermission = PlayerSettings.Android.forceInternetPermission;
		settings.AndroidForceSDCardPermission = PlayerSettings.Android.forceSDCardPermission;

		settings.AndroidShowProgressBarInLoadingScreen = PlayerSettings.Android.showActivityIndicatorOnLoading.ToString();



		// BlackBerry only build settings
		// ---------------------------------------------------------------

		//settings.BlackBerryBuildSubtarget = EditorUserBuildSettings.blackberryBuildSubtarget.ToString();
		//settings.BlackBerryBuildType = EditorUserBuildSettings.blackberryBuildType.ToString();
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)
					
#else
		settings.BlackBerryAuthorID = PlayerSettings.BlackBerry.authorId;
#endif
		

// 		settings.BlackBerryDeviceAddress = PlayerSettings.BlackBerry.deviceAddress;
// 
// 		settings.BlackBerrySaveLogPath = PlayerSettings.BlackBerry.saveLogPath;
// 		settings.BlackBerryTokenPath = PlayerSettings.BlackBerry.tokenPath;
// 
// 		settings.BlackBerryTokenAuthor = PlayerSettings.BlackBerry.tokenAuthor;
// 		settings.BlackBerryTokenExpiration = PlayerSettings.BlackBerry.tokenExpires;
// 
// 		settings.BlackBerryHasCamPermissions = PlayerSettings.BlackBerry.HasCameraPermissions();
// 		settings.BlackBerryHasMicPermissions = PlayerSettings.BlackBerry.HasMicrophonePermissions();
// 		settings.BlackBerryHasGpsPermissions = PlayerSettings.BlackBerry.HasGPSPermissions();
// 		settings.BlackBerryHasIdPermissions = PlayerSettings.BlackBerry.HasIdentificationPermissions();
// 		settings.BlackBerryHasSharedPermissions = PlayerSettings.BlackBerry.HasSharedPermissions();
	}


	public static void PopulateBigConsoleGen07Settings(UnityBuildSettings settings)
	{
		// XBox 360 build settings
		// ---------------------------------------------------------------

		settings.Xbox360BuildSubtarget = EditorUserBuildSettings.xboxBuildSubtarget.ToString();
		settings.Xbox360RunMethod = EditorUserBuildSettings.xboxRunMethod.ToString();

		settings.Xbox360TitleId = PlayerSettings.xboxTitleId;
		settings.Xbox360ImageXexFilePath = PlayerSettings.xboxImageXexFilePath;
		settings.Xbox360SpaFilePath = PlayerSettings.xboxSpaFilePath;

		settings.Xbox360AutoGenerateSpa = PlayerSettings.xboxGenerateSpa;
		settings.Xbox360DeployKinectResources = PlayerSettings.xboxDeployKinectResources;
		settings.Xbox360EnableKinect = PlayerSettings.xboxEnableKinect;
		settings.Xbox360EnableKinectAutoTracking = PlayerSettings.xboxEnableKinectAutoTracking;
		settings.Xbox360EnableSpeech = PlayerSettings.xboxEnableSpeech;
		settings.Xbox360EnableAvatar = PlayerSettings.xboxEnableAvatar;

		settings.Xbox360SpeechDB = PlayerSettings.xboxSpeechDB;

		settings.Xbox360AdditionalTitleMemSize = PlayerSettings.xboxAdditionalTitleMemorySize;

		settings.Xbox360DeployKinectHeadOrientation = PlayerSettings.xboxDeployKinectHeadOrientation;
		settings.Xbox360DeployKinectHeadPosition = PlayerSettings.xboxDeployKinectHeadPosition;



		// Playstation devices build settings
		// ---------------------------------------------------------------

		settings.SCEBuildSubtarget = EditorUserBuildSettings.sceBuildSubtarget.ToString();

		// PS3 build settings
		// ---------------------------------------------------------------

		// paths
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)
			
			
#else		
		settings.PS3TitleConfigFilePath = PlayerSettings.ps3TitleConfigPath;
		settings.PS3DLCConfigFilePath = PlayerSettings.ps3DLCConfigPath;
		settings.PS3ThumbnailFilePath = PlayerSettings.ps3ThumbnailPath;
		settings.PS3BackgroundImageFilePath = PlayerSettings.ps3BackgroundPath;
		settings.PS3BackgroundSoundFilePath = PlayerSettings.ps3SoundPath;
		settings.PS3TrophyPackagePath = PlayerSettings.ps3TrophyPackagePath;
		settings.PS3InTrialMode = PlayerSettings.ps3TrialMode;
#endif

		settings.PS3VideoMemoryForVertexBuffers = PlayerSettings.PS3.videoMemoryForVertexBuffers;
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)
			
			
#else	
		settings.PS3BootCheckMaxSaveGameSizeKB = PlayerSettings.ps3BootCheckMaxSaveGameSizeKB;

		settings.PS3SaveGameSlots = PlayerSettings.ps3SaveGameSlots;

		settings.PS3NpCommsId = PlayerSettings.ps3TrophyCommId;
		settings.PS3NpCommsSig = PlayerSettings.ps3TrophyCommSig;




		// PS Vita build settings
		// ---------------------------------------------------------------

		settings.PSVTrophyPackagePath = PlayerSettings.psp2NPTrophyPackPath;
		settings.PSVParamSfxPath = PlayerSettings.psp2ParamSfxPath;

		settings.PSVNpCommsId = PlayerSettings.psp2NPCommsID;
		settings.PSVNpCommsSig = PlayerSettings.psp2NPCommsSig;
#endif
	}
}

}
