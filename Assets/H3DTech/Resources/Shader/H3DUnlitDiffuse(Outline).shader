// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "H3D/InGame/Character/Unlit/Diffuse(Outline)" {
Properties {

	_MainTex ("Main Texture", 2D) = "white" {}
	_Color("Main Color" ,color) = (0.8,0.8,0.8,1.0)
	_SHLightingScale("Ambient LightProbe Effect",Range(0.0,5.0)) = 2
	
	//OUTLINE
	_Outline ("描点宽度", Range(0,0.05)) = 0.005
	_OutlineColor ("描边颜色", Color) = (0.2, 0.2, 0.2, 1)
}
Category {
	  
    Tags { "Queue" = "Geometry" "IgnoreProjector" = "True"  }
	
	CGINCLUDE
    #include "H3DFramework.cginc"
    struct v2f {
        float4 pos : SV_POSITION;
        float2 muv : TEXCOORD0;
        H3D_AMBIENT_LIGHT_COODS(2)
    };
    sampler2D _MainTex;
    half4     _MainTex_ST;
    fixed3    _Color;
    ENDCG 
    
	SubShader {
	  LOD 200
	   UsePass "H3D/InGame/Character/Unlit/Diffuse/SHLIGHTING"
	   UsePass "Hidden/H3D/H3D-Outline/OUTLINE"
	  } 
      FallBack "H3D/InGame/SceneDynamicObjects/SimpleDiffuse(VertexLit shader cast shadow)"
   }
}
