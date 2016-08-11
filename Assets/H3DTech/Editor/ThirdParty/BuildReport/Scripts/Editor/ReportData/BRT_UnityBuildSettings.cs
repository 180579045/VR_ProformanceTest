using UnityEngine;
using System.Collections;

namespace BuildReportTool
{

[System.Serializable]
public class UnityBuildSettings
{
	public string CompanyName; // PlayerSettings.companyName
	public string ProductName; // PlayerSettings.productName

	public bool UsingAdvancedLicense; // PlayerSettings.advancedLicense / Application.HasProLicense() ?

	public bool InstallInBuildFolder; // EditorUserBuildSettings.installInBuildFolder




	// debug settings
	// ---------------------------------------------------------------

	public bool EnableDevelopmentBuild; // EditorUserBuildSettings.development / Debug.isDebugBuild

	public bool EnableDebugLog; // PlayerSettings.usePlayerLog

	public bool ConnectProfiler; // EditorUserBuildSettings.connectProfiler

	public bool EnableSourceDebugging; // EditorUserBuildSettings.allowDebugging



	// code settings
	// ---------------------------------------------------------------

	public string[] CompileDefines; // EditorUserBuildSettings.activeScriptCompilationDefines

	public string StrippingLevelUsed; // PlayerSettings.strippingLevel

	public string NETApiCompatibilityLevel; // PlayerSettings.apiCompatibilityLevel

	public string AOTOptions; // PlayerSettings.aotOptions

	public bool EnableExplicitNullChecks; // EditorUserBuildSettings.explicitNullChecks

#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)
			
#else	
	public bool StripPhysicsCode; // in Unity 4: PlayerSettings.stripPhysics
#endif

	
	// rendering settings
	// ---------------------------------------------------------------

	public string ColorSpaceUsed; // PlayerSettings.colorSpace

	public bool StripUnusedMeshComponents; // PlayerSettings.stripUnusedMeshComponents
	public bool UseMultithreadedRendering; // PlayerSettings.MTRendering

	// in Unity 3: only xbox 360 has this with PlayerSettings.xboxSkinOnGPU
	// in Unity 4, this is PlayerSettings.gpuSkinning
	public bool UseGPUSkinning;

	public string RenderingPathUsed; // Unity 4: PlayerSettings.renderingPath








	// web player settings
	// ---------------------------------------------------------------

	public int WebPlayerDefaultScreenWidth; // PlayerSettings.defaultWebScreenWidth
	public int WebPlayerDefaultScreenHeight; // PlayerSettings.defaultWebScreenHeight

	public bool WebPlayerEnableStreaming; // EditorUserBuildSettings.webPlayerStreamed
	public bool WebPlayerDeployOffline; // EditorUserBuildSettings.webPlayerOfflineDeployment

	public int WebPlayerFirstStreamedLevelWithResources; // PlayerSettings.firstStreamedLevelWithResources



#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)
			
#else
	// flash player settings
	// ---------------------------------------------------------------
	public string FlashBuildSubtarget; // EditorUserBuildSettings.flashBuildSubtarget
#endif


	// shared by web and desktop
	// ---------------------------------------------------------------
	public bool RunInBackground; // PlayerSettings.runInBackground




	// desktop (windows/mac/linux) build settings
	// ---------------------------------------------------------------

	public string StandaloneResolutionDialogSettingUsed; // PlayerSettings.displayResolutionDialog

	public int StandaloneDefaultScreenWidth; // PlayerSettings.defaultScreenWidth
	public int StandaloneDefaultScreenHeight; // PlayerSettings.defaultScreenHeight

	public bool StandaloneFullScreenByDefault; // PlayerSettings.defaultIsFullScreen

	public bool StandaloneCaptureSingleScreen; // PlayerSettings.captureSingleScreen

	public bool StandaloneForceSingleInstance; // Unity 4: PlayerSettings.forceSingleInstance
	public bool StandaloneEnableResizableWindow; // Unity 4: PlayerSettings.resizableWindow


	public bool StandaloneUseStereoscopic3d; // PlayerSettings.stereoscopic3D


	// windows only build settings
	// ---------------------------------------------------------------

	public bool WinUseDirect3D11IfAvailable; // Unity 4: PlayerSettings.useDirect3D11



	// mac only build settings
	// ---------------------------------------------------------------

	public bool MacUseAppStoreValidation; // PlayerSettings.useMacAppStoreValidation
	public string MacFullscreenModeUsed; // PlayerSettings.macFullscreenMode







	// Mobile build settings
	// ---------------------------------------------------------------

	public string MobileBundleIdentifier; // PlayerSettings.bundleIdentifier ("Bundle Identifier" in iOS, "Package Identifier" in Android)
	public string MobileBundleVersion; // PlayerSettings.bundleVersion ("Bundle Version" in iOS, "Version Name" in Android)
	public bool MobileHideStatusBar; // PlayerSettings.statusBarHidden

	public int MobileAccelerometerFrequency; // PlayerSettings.accelerometerFrequency

	public string MobileDefaultOrientationUsed; // PlayerSettings.defaultInterfaceOrientation
	public bool MobileEnableAutorotateToPortrait; // PlayerSettings.allowedAutorotateToPortrait
	public bool MobileEnableAutorotateToReversePortrait; // PlayerSettings.allowedAutorotateToPortraitUpsideDown
	public bool MobileEnableAutorotateToLandscapeLeft; // PlayerSettings.allowedAutorotateToLandscapeLeft
	public bool MobileEnableAutorotateToLandscapeRight; // PlayerSettings.allowedAutorotateToLandscapeRight
	public bool MobileEnableOSAutorotation; // PlayerSettings.useOSAutorotation

	public bool Use32BitDisplayBuffer; // PlayerSettings.use32BitDisplayBuffer



	// iOS only build settings
	// ---------------------------------------------------------------
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)
			
#else
	public bool iOSAppendedToProject; // EditorUserBuildSettings.appendProject
#endif

	public bool iOSSymlinkLibraries; // EditorUserBuildSettings.symlinkLibraries

	public string iOSAppDisplayName; // PlayerSettings.iOS.applicationDisplayName

	public string iOSScriptCallOptimizationUsed; // PlayerSettings.iOS.scriptCallOptimization

	public string iOSSDKVersionUsed; // PlayerSettings.iOS.sdkVersion
	public string iOSTargetOSVersion; // PlayerSettings.iOS.targetOSVersion

	public string iOSTargetDevice; // PlayerSettings.iOS.targetDevice
	public string iOSTargetResolution; // PlayerSettings.iOS.targetResolution

	public bool iOSIsIconPrerendered; // PlayerSettings.iOS.prerenderedIcon

	public string iOSRequiresPersistentWiFi; // PlayerSettings.iOS.requiresPersistentWiFi

	public string iOSStatusBarStyle; // PlayerSettings.iOS.statusBarStyle
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)		
#else
	public bool iOSExitOnSuspend; // PlayerSettings.iOS.exitOnSuspend
#endif

	public string iOSShowProgressBarInLoadingScreen; // PlayerSettings.iOS.showActivityIndicatorOnLoading


	// Android only build settings
	// ---------------------------------------------------------------

	public string AndroidBuildSubtarget; // EditorUserBuildSettings.androidBuildSubtarget

	public bool AndroidUseAPKExpansionFiles; // PlayerSettings.Android.useAPKExpansionFiles

	public bool AndroidCreateProject;

	public bool AndroidUseLicenseVerification; // PlayerSettings.Android.licenseVerification

#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)		
#else
	public bool AndroidUse24BitDepthBuffer; // PlayerSettings.Android.use24BitDepthBuffer
#endif

	public int AndroidVersionCode; // PlayerSettings.Android.bundleVersionCode

	public string AndroidMinSDKVersion; // PlayerSettings.Android.minSdkVersion
	public string AndroidTargetDevice; // PlayerSettings.Android.targetDevice

	public string AndroidSplashScreenScaleMode; // PlayerSettings.Android.splashScreenScale

	public string AndroidPreferredInstallLocation; // PlayerSettings.Android.preferredInstallLocation

	public bool AndroidForceInternetPermission; // PlayerSettings.Android.forceInternetPermission
	public bool AndroidForceSDCardPermission; // PlayerSettings.Android.forceSDCardPermission

	public string AndroidShowProgressBarInLoadingScreen; // PlayerSettings.Android.showActivityIndicatorOnLoading

	public string AndroidKeyAliasName; // PlayerSettings.Android.keyaliasName
	public string AndroidKeystoreName; // PlayerSettings.Android.keystoreName




	// BlackBerry only build settings
	// ---------------------------------------------------------------

	public string BlackBerryBuildSubtarget; // EditorUserBuildSettings.blackberryBuildSubtarget
	public string BlackBerryBuildType; // EditorUserBuildSettings.blackberryBuildType
#if (UNITY_5||UNITY_5_1||UNITY_5_1_0)
		
#else
	public string BlackBerryAuthorID; // PlayerSettings.BlackBerry.authorId
#endif
	public string BlackBerryDeviceAddress; // PlayerSettings.BlackBerry.deviceAddress

	public string BlackBerrySaveLogPath; // PlayerSettings.BlackBerry.saveLogPath
	public string BlackBerryTokenPath; // PlayerSettings.BlackBerry.tokenPath

	public string BlackBerryTokenAuthor; // PlayerSettings.BlackBerry.tokenAuthor
	public string BlackBerryTokenExpiration; // PlayerSettings.BlackBerry.tokenExpires

	public bool BlackBerryHasCamPermissions; // PlayerSettings.BlackBerry.HasCameraPermissions()
	public bool BlackBerryHasMicPermissions; // PlayerSettings.BlackBerry.HasMicrophonePermissions()
	public bool BlackBerryHasGpsPermissions; // PlayerSettings.BlackBerry.HasGPSPermissions()
	public bool BlackBerryHasIdPermissions; // PlayerSettings.BlackBerry.HasIdentificationPermissions()
	public bool BlackBerryHasSharedPermissions; // PlayerSettings.BlackBerry.HasSharedPermissions()







	// XBox 360 build settings
	// ---------------------------------------------------------------

	public string Xbox360BuildSubtarget; // EditorUserBuildSettings.xboxBuildSubtarget
	public string Xbox360RunMethod; // EditorUserBuildSettings.xboxRunMethod

	public string Xbox360TitleId; // PlayerSettings.xboxTitleId

	public string Xbox360ImageXexFilePath; // PlayerSettings.xboxImageXexFilePath
	public string Xbox360SpaFilePath; // PlayerSettings.xboxSpaFilePath

	public bool Xbox360AutoGenerateSpa; // PlayerSettings.xboxGenerateSpa
	public bool Xbox360DeployKinectResources; // PlayerSettings.xboxDeployKinectResources

	public bool Xbox360EnableKinect; // PlayerSettings.xboxEnableKinect
	public bool Xbox360EnableKinectAutoTracking; // PlayerSettings.xboxEnableKinectAutoTracking
	public bool Xbox360EnableSpeech; // PlayerSettings.xboxEnableSpeech
	public bool Xbox360EnableAvatar; // PlayerSettings.xboxEnableAvatar

	public uint Xbox360SpeechDB; // PlayerSettings.xboxSpeechDB

	public int Xbox360AdditionalTitleMemSize; // PlayerSettings.xboxAdditionalTitleMemorySize

	public bool Xbox360DeployKinectHeadOrientation; // PlayerSettings.xboxDeployKinectHeadOrientation
	public bool Xbox360DeployKinectHeadPosition; // PlayerSettings.xboxDeployKinectHeadPosition




	// Playstation devices build settings
	// ---------------------------------------------------------------

	public string SCEBuildSubtarget; // EditorUserBuildSettings.sceBuildSubtarget


	// PS3 build settings
	// ---------------------------------------------------------------

	// paths
	public string PS3TitleConfigFilePath; // PlayerSettings.ps3TitleConfigPath
	public string PS3DLCConfigFilePath; // PlayerSettings.ps3DLCConfigPath
	public string PS3ThumbnailFilePath; // PlayerSettings.ps3ThumbnailPath
	public string PS3BackgroundImageFilePath; // PlayerSettings.ps3BackgroundPath
	public string PS3BackgroundSoundFilePath; // PlayerSettings.ps3SoundPath
	public string PS3TrophyPackagePath; // PlayerSettings.ps3TrophyPackagePath

	public bool PS3InTrialMode; // PlayerSettings.ps3TrialMode

	public int PS3VideoMemoryForVertexBuffers; // PlayerSettings.PS3.videoMemoryForVertexBuffers
	public int PS3BootCheckMaxSaveGameSizeKB; // PlayerSettings.ps3BootCheckMaxSaveGameSizeKB

	public int PS3SaveGameSlots; // PlayerSettings.ps3SaveGameSlots

	public string PS3NpCommsId; // PlayerSettings.ps3TrophyCommId
	public string PS3NpCommsSig; // PlayerSettings.ps3TrophyCommSig




	// PS Vita build settings
	// ---------------------------------------------------------------

	public string PSVTrophyPackagePath; // PlayerSettings.psp2NPTrophyPackPath
	public string PSVParamSfxPath; // PlayerSettings.psp2ParamSfxPath


	public string PSVNpCommsId; // PlayerSettings.psp2NPCommsID
	public string PSVNpCommsSig; // PlayerSettings.psp2NPCommsSig





	// Derived Values
	// ---------------------------------------------------------------


	public bool HasValues
	{
		get
		{
			return !string.IsNullOrEmpty(CompanyName) && !string.IsNullOrEmpty(NETApiCompatibilityLevel);
		}
	}
}

}
