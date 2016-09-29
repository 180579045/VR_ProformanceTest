// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "H3D/InGame/SceneStaticObjects/ReflectiveSpecular" {
	Properties 
	{
		_MainTex ("(RGB)(A反射率)", 2D) = "white" {}
		_Color("主色调",Color) = (1.0,1.0,1.0,1.0)
		_SpecColor ("高光颜色", Color) = (1.0,1.0,1.0,1.0)
		_Shininess ("高光强度", Range(0.0,1.0)) = 0.7
		_ReflTex ("反射纹理", 2D) = "white" {}
		_ReflColor ("反射颜色", Color) = (1.0,1.0,1.0,1.0)
		_ReflPower ("反射强度",Range(0.0,1.0)) = 1.0
		_ReflAnimFactor("反射纹理坐标动画权重",Range(0.0,1.0)) = 0.0
		_ReflTexUOffsetSpeed ("反射偏移水平速度",float) = 0
		_ReflTexVOffsetSpeed ("反射偏移纵向速度",float) = 0
	}
	Category {
        Tags { "Queue" = "Geometry"} 
        CGINCLUDE
        #include "H3DFramework.cginc"
        sampler2D _MainTex;
		sampler2D _ReflTex;
		half4     _ReflColor;
		half      _ReflPower;
		half      _ReflAnimFactor;
		half      _ReflTexUOffsetSpeed;
		half      _ReflTexVOffsetSpeed;
		fixed4    _Color;
		ENDCG
	 	SubShader {
			LOD 400
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
				  half2  ruv          : TEXCOORD1;
				  fixed3 worldViewDir : TEXCOORD2;
				  fixed3 worldNormal  : TEXCOORD3;
				  LIGHTING_COORDS(4,5) 
				  #ifndef LIGHTMAP_OFF
				  half2 lightmap_uv: TEXCOORD6;
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
					o.ruv = H3DObjSpaceSphereMapUV(v.vertex,v.normal);
					
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					float3 viewDirForLight = UnityWorldSpaceViewDir(worldPos);
                    float3 worldLightDir   = UnityWorldSpaceLightDir(worldPos);
                    viewDirForLight = normalize(normalize(viewDirForLight) + normalize(worldLightDir));
                    o.worldViewDir = viewDirForLight;
                       
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
					fixed4 rc = tex2D(_ReflTex,i.ruv + reflUVOff );
  					fixed3 worldViewDir = normalize(i.worldViewDir);
					fixed  atten = LIGHT_ATTENUATION(i);
					
					SurfaceOutput o;
					UNITY_INITIALIZE_OUTPUT(SurfaceOutput,o);
					
					o.Albedo   = lerp (mc.rgb * _Color  , rc.rgb*_ReflColor.rgb*rc.a*mc.a, _ReflPower );
                    o.Normal = normalize( i.worldNormal);
					o.Specular = _Shininess;
					o.Gloss    = mc.a;
			 
					fixed4 c = 0;
					c += H3DBlinnPhongAddHalf(o, worldViewDir,atten);
					#ifndef LIGHTMAP_OFF
	           		      c.rgb =  2 * c.rgb * DecodeLightmap(H3D_SAMPLE_TEX2D(unity_Lightmap, i.lightmap_uv));
                    #endif
					return c;
				} 
				ENDCG
			}//end Lod2
	} 
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
			  half2  ruv          : TEXCOORD1;
			  LIGHTING_COORDS(2,3) 
			  #ifndef LIGHTMAP_OFF
			  half2 lightmap_uv: TEXCOORD4;
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
				o.ruv = H3DObjSpaceSphereMapUV(v.vertex,v.normal);
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
				fixed4 rc = tex2D(_ReflTex,i.ruv + reflUVOff );
				fixed  atten = LIGHT_ATTENUATION(i);
			    fixed4 c = 0;
			    c.rgb   = lerp (mc.rgb * _Color  , rc.rgb*_ReflColor.rgb*rc.a*mc.a, _ReflPower ) * atten;

				#ifndef LIGHTMAP_OFF
           		       c.rgb  = 2 * c.rgb * DecodeLightmap(H3D_SAMPLE_TEX2D(unity_Lightmap, i.lightmap_uv));
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
			  LIGHTING_COORDS(2,3) 
			  #ifndef LIGHTMAP_OFF
			  half2 lightmap_uv: TEXCOORD4;
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
				c.rgb   = mc.rgb * atten;
				#ifndef LIGHTMAP_OFF
           		       c.rgb  = 2*c.rgb * DecodeLightmap(H3D_SAMPLE_TEX2D(unity_Lightmap, i.lightmap_uv));
                #endif
				return c;
			} 
			ENDCG
		}//end Lod2
	} 
	
	   FallBack "H3D/InGame/SceneStaticObjects/SimpleDiffuse (Supports Lightmap)"
	 }//end Category
}
