Shader "H3D/InGame/SceneDynamicObjects/ReflectiveSurfReflectivityTransparentAdditive" {
	Properties 
	{
		_MainTex ("(RGB)(A反射率)", 2D) = "white" {}
		_ReflTex ("反射纹理", 2D) = "white" {}
		_ReflColor ("反射颜色", Color) = (1.0,1.0,1.0,1.0)
		_ReflPower ("反射强度",Range(0.0,1.0)) = 1.0
		_Alpha("透明度",Range(0.0,1.0)) = 1.0
		_ReflAnimFactor("反射纹理坐标动画权重",Range(0.0,1.0)) = 0.0
		_ReflTexUOffsetSpeed ("反射偏移水平速度",float) = 0
		_ReflTexVOffsetSpeed ("反射偏移纵向速度",float) = 0
	}

    Category {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector" = "True"  } 
        Blend SrcAlpha OneMinusSrcAlpha
        CGINCLUDE
        #include "H3DFramework.cginc"
    	sampler2D _MainTex;
		sampler2D _ReflTex;
		half4     _ReflColor;
		half      _ReflPower;
		half      _Alpha;
		half      _ReflAnimFactor;
		half      _ReflTexUOffsetSpeed;
		half      _ReflTexVOffsetSpeed;
		
		struct Input 
		{
			half2 uv_MainTex;
			half3 worldRefl;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			float realTime = _Time.y;
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
//		    half3 worldRefl = WorldReflectionVector (IN, o.Normal);
			IN.worldRefl = normalize(IN.worldRefl);  		
			half2 reflUVOff = half2
			(
				fmod( realTime*_ReflTexUOffsetSpeed * _ReflAnimFactor , 1.0)  , 
				fmod( realTime*_ReflTexVOffsetSpeed * _ReflAnimFactor , 1.0)
			);
			o.Albedo = tex2D(_ReflTex,IN.worldRefl.xy * 0.5f + 0.5f + reflUVOff ).xyz * ( c.a * _ReflPower) * _ReflColor.xyz;
			o.Albedo += c.xyz;
			o.Alpha = _Alpha * c.a;
			
		}
		ENDCG
		SubShader {  
			LOD 400
			pass { 
				ZWrite On ZTest LEqual ColorMask 0 
			}
			CGPROGRAM
			#pragma surface surf H3DLambert alpha:blend exclude_path:prepass  nolightmap noforwardadd 
            ENDCG
		} 
		
		SubShader {  
			LOD 300
			Pass {
				Tags { "LightMode" = "ForwardBase" }
				CGPROGRAM
				#pragma vertex vert_surf
				#pragma fragment frag_surf
				half4     _MainTex_ST;
				struct v2f {
				  float4 pos       : SV_POSITION;
				  half2  uv        : TEXCOORD0;
				  half3  worldRefl : TEXCOORD1;
				  H3D_LIGHT_COODS(2)
				};
				
				v2f vert_surf (appdata_full v)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.uv  = TRANSFORM_TEX(v.texcoord, _MainTex);
					H3D_LAMBERT_LIGHT(o);
					fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
					o.worldRefl = reflect(-worldViewDir, worldNormal);
					return o;
				}
				fixed4 frag_surf (v2f i) : SV_Target
				{
					Input surfInput;
					surfInput.uv_MainTex =i.uv;
					surfInput.worldRefl  =i.worldRefl;
					SurfaceOutput o;
					UNITY_INITIALIZE_OUTPUT(SurfaceOutput,o);
					surf (surfInput, o);
					fixed4 c = 0;
					c.rgb =  H3D_SURFACE_LIGHT(i,o);
					c.a   =  o.Alpha * c.a;
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
				#pragma vertex vert_surf
				#pragma fragment frag_surf
				half4 _MainTex_ST;
				struct v2f {
				  float4 pos       : SV_POSITION;
				  half2  uv     : TEXCOORD0;
				  H3D_LIGHT_COODS(1)
				};
				
				void Lod3_surf (Input IN, inout SurfaceOutput o) 
		        {
					half4 c  = tex2D (_MainTex, IN.uv_MainTex);
					o.Albedo = c.xyz + c.xyz* _ReflPower * _ReflColor.xyz;
					o.Alpha  = _Alpha * c.a;
		        }
				v2f vert_surf (appdata_full v)
				{
					v2f o;
					o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					H3D_LAMBERT_LIGHT(o);
					return o;
				}
				fixed4 frag_surf (v2f i) : SV_Target
				{
					Input surfInput;
					surfInput.uv_MainTex =i.uv;
					SurfaceOutput o; 
					UNITY_INITIALIZE_OUTPUT(SurfaceOutput,o);
					Lod3_surf (surfInput, o);
					fixed4 c = 0;
					c.rgb =  H3D_SURFACE_LIGHT(i,o);
					c.a   =  o.Alpha;
					return c;
				} 
				ENDCG
			}
		}//Lod3 
	}//end Categroy
}
