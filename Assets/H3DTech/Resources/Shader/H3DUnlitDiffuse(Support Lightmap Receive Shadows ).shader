//   Unlit shader
// - no lighting
// - support lightmap 
// - support recevie shadows 
// - per-material color

Shader "H3D/InGame/SceneStaticObjects/Diffuse" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Color("Main Color", Color) = (1,1,1,1.0)
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200
	
	Pass {
	  	Tags { "LightMode"="ForwardBase"} 
		CGPROGRAM
		    #pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag
            #include "H3DFramework.cginc"
			struct v2f {
				float4 pos : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				LIGHTING_COORDS(1,2)  
				#ifdef LIGHTMAP_ON
				half2 lightmap_uv: TEXCOORD3;
				#endif
			};


			sampler2D _MainTex;
			half4     _MainTex_ST;
			fixed4    _Color;
			#ifdef LIGHTMAP_ON
			H3D_LightMap_Declare 
       		#endif
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				#ifdef LIGHTMAP_ON
            	o.lightmap_uv = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
            	#endif
				TRANSFER_VERTEX_TO_FRAGMENT(o);  
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				fixed attenuation = LIGHT_ATTENUATION(i);
				#ifdef LIGHTMAP_ON
           		  col.rgb *= DecodeLightmap(H3D_SAMPLE_TEX2D(unity_Lightmap, i.lightmap_uv));
            	#endif 
				return attenuation*col*_Color;
			}
		ENDCG
	}
	
  }
 FallBack "H3D/InGame/SceneStaticObjects/SimpleDiffuse (Supports Lightmap)"
}