
Shader "H3D/InGame/SceneStaticObjects/T4M/T4M 2 Textures Unlit LM" {
Properties {
    _Splat0 ("Layer1 (RGB)", 2D) = "white" {}
	_Splat1 ("Layer2 (RGB)", 2D) = "white" {}
	_Control ("Control (RGBA)", 2D) = "white" {}
	_MainTex ("Never Used", 2D) = "white" {}
}
SubShader {
	LOD 200
	
    Pass {

		Tags { "LightMode"="ForwardBase" } 
		
	    CGPROGRAM  
        #pragma multi_compile_fwdbase     
        #pragma vertex vert  
        #pragma fragment frag  
        #include "H3DFramework.cginc"
		sampler2D _Splat0 ;
		sampler2D _Splat1 ;
		sampler2D _Control;

		struct v2f {
			float4  pos : SV_POSITION;
			#ifdef LIGHTMAP_ON
			float2  uv[4] : TEXCOORD0;
			LIGHTING_COORDS(5,6)  
			#endif
			#ifdef LIGHTMAP_OFF
			float2  uv[3] : TEXCOORD0;
			LIGHTING_COORDS(4,5)  
			#endif
		};

		float4 _Splat0_ST;
		float4 _Splat1_ST;;
		float4 _Control_ST;
		#ifdef LIGHTMAP_ON
 		H3D_LightMap_Declare 
        #endif
		v2f vert (appdata_full v)
		{
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv[0] = TRANSFORM_TEX (v.texcoord, _Splat0);
			o.uv[1] = TRANSFORM_TEX (v.texcoord, _Splat1);
			o.uv[2] = TRANSFORM_TEX (v.texcoord, _Control);
			#ifdef LIGHTMAP_ON
            	o.uv[3] = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            #endif
            
            TRANSFER_VERTEX_TO_FRAGMENT(o);  
			return o;
		}

		fixed4 frag (v2f i) : COLOR
		{
			fixed2 Mask = tex2D( _Control, i.uv[2].xy ).rg;
			fixed3 lay1 = tex2D( _Splat0, i.uv[0].xy );
			fixed3 lay2 = tex2D( _Splat1, i.uv[1].xy );
   				
    		float  attenuation = LIGHT_ATTENUATION(i);
    		fixed4 c = 1;
			c.xyz = (lay1.xyz * Mask.r + lay2.xyz * Mask.g)*attenuation;
			#ifdef LIGHTMAP_ON
            	c.rgb *= DecodeLightmap(H3D_SAMPLE_TEX2D(unity_Lightmap, i.uv[3]));
            #endif
			c.w = 0;
			return c;
		}
		ENDCG
    }
	

  }   
 FallBack "H3D/InGame/SceneStaticObjects/SimpleDiffuse (Supports Lightmap)"
} 