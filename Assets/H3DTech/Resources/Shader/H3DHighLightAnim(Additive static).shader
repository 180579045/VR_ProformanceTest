Shader "H3D/InGame/SceneStaticObjects/HighLightAnimAdditive" {
	Properties 
	{
		_MainTex ("主纹理", 2D) = "white" {}
		_Color("主颜色",color) =  (1.0,1.0,1.0,1.0)
		_ReflTex ("高光纹理", 2D) = "black" {}
		_HighLightColor ("高光颜色",Color) = (1.0,1.0,1.0,1.0)
		_ReflPower ("高光强度",Range(0.0,1.0)) = 1.0
		_ReflAnimFactor("高光纹理坐标动画权重",Range(0.0,1.0)) = 0.0
		_ReflTexUOffsetSpeed ("高光偏移水平速度",float) = 0
		_ReflTexVOffsetSpeed ("高光偏移纵向速度",float) = 0
	}
	Category {
        Tags { "Queue" = "Geometry"}
	    CGINCLUDE
        #include "H3DFramework.cginc"
		sampler2D _MainTex;
		sampler2D _ReflTex;
		
		half4     _HighLightColor;
		half      _ReflPower;
		half      _ReflAnimFactor;
		half      _ReflTexUOffsetSpeed;
		half      _ReflTexVOffsetSpeed;
		fixed4     _Color;
		ENDCG
	
        SubShader {
		LOD 300
		Pass {
		    
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma multi_compile_fwdbase 
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			half4     _MainTex_ST;
			struct v2f {
			  float4 pos          : SV_POSITION;
			  half2  uv           : TEXCOORD0;
			  LIGHTING_COORDS(1,2) 
			  #ifndef LIGHTMAP_OFF
			  half2 lightmap_uv: TEXCOORD3;
			  #endif
			};
			#ifndef LIGHTMAP_OFF
	        H3D_LightMap_Declare 
            #endif
			
			v2f vert_surf (appdata_full v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
                #ifndef LIGHTMAP_OFF
        	    o.lightmap_uv = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
        	    #endif
        	    TRANSFER_VERTEX_TO_FRAGMENT(o); 
				return o;
			}
			fixed4 frag_surf (v2f i) : SV_Target
			{
			    
				float realTime = _Time.y;
                fixed4 mc = tex2D (_MainTex, i.uv);
                half2 reflUVOff = half2(
				fmod( realTime*_ReflTexUOffsetSpeed * _ReflAnimFactor , 1.0 ), 
				fmod( realTime*_ReflTexVOffsetSpeed * _ReflAnimFactor , 1.0 )
				);
				fixed4 rc = tex2D(_ReflTex,i.uv + reflUVOff );
				fixed  atten = LIGHT_ATTENUATION(i);
			    fixed4 c = 0;
			    c.rgb   = (mc.rgb * _Color.rgb + rc.rgb * rc.a*mc.a*_ReflPower * _HighLightColor.xyz)*atten;
				#ifndef LIGHTMAP_OFF
           		 c.rgb  = c.rgb * DecodeLightmap(H3D_SAMPLE_TEX2D(unity_Lightmap, i.lightmap_uv));
                #endif
				return c;
			} 
			ENDCG
		}//end Lod2
	} 
		
		
		
    SubShader {
		LOD 200
		Pass {
		    
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma multi_compile_fwdbase 
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			half4     _MainTex_ST;
			struct v2f {
			  float4 pos          : SV_POSITION;
			  half2  uv           : TEXCOORD0;
			  LIGHTING_COORDS(1,2) 
			  #ifndef LIGHTMAP_OFF
			  half2 lightmap_uv: TEXCOORD3;
			  #endif
			};
			#ifndef LIGHTMAP_OFF
	        H3D_LightMap_Declare 
            #endif
			
			v2f vert_surf (appdata_full v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
                #ifndef LIGHTMAP_OFF
        	    o.lightmap_uv = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
        	    #endif
        	    TRANSFER_VERTEX_TO_FRAGMENT(o); 
				return o;
			}
			fixed4 frag_surf (v2f i) : SV_Target
			{
                fixed4 mc = tex2D (_MainTex, i.uv);
				fixed  atten = LIGHT_ATTENUATION(i);
			    fixed4 c = 0;
				c.rgb   = mc.rgb * _Color.rgb * atten;
				#ifndef LIGHTMAP_OFF
           		c.rgb  = c.rgb * DecodeLightmap(H3D_SAMPLE_TEX2D(unity_Lightmap, i.lightmap_uv));
                #endif
				return c;
			} 
			ENDCG
		}//end Lod2
	} 
	
	   FallBack "H3D/InGame/SceneStaticObjects/SimpleDiffuse (Supports Lightmap)"
	 }//end Category

}
