
Shader "H3D/InGame/SceneStaticObjects/BumpDiffuse(support lightmap)" {
	Properties {
		
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_BumpScale("BumpScale",float) = 1
	}
	Category{
	
	    Tags { "Queue" = "Geometry"}
        
        CGINCLUDE
        #include "H3DFramework.cginc"
        sampler2D    _MainTex;
		sampler2D    _BumpMap;
		half4        _MainTex_ST;
		half4        _BumpMap_ST;
		half         _BumpScale;
	
		
        ENDCG
        
		SubShader {
		    LOD 300
		    
		    pass {
			Tags { "LightMode" = "ForwardBase" }
			
			CGPROGRAM
			#pragma multi_compile_fwdbase
	        #pragma vertex Lod1_vert
	        #pragma fragment Lod1_frag
	        #pragma target 3.0
	        
			struct v2f_Lod1 {
	            float4 pos         : SV_POSITION;
	            half4  pack0       : TEXCOORD0;
	            fixed4 tSpace0     : TEXCOORD1;
                fixed4 tSpace1     : TEXCOORD2;
                fixed4 tSpace2     : TEXCOORD3;
	            SHADOW_COORDS(4)  
	            #if LIGHTMAP_ON
	            half2 lightmap_uv  : TEXCOORD5; 
	            #endif  
		    };
		    
		    #if LIGHTMAP_ON
		    H3D_LightMap_Declare 
			#endif  
			
			v2f_Lod1 Lod1_vert (appdata_full v)
		    {
	            v2f_Lod1 o;
	            UNITY_INITIALIZE_OUTPUT(v2f_Lod1,o);
	            o.pos =  mul (UNITY_MATRIX_MVP, v.vertex);
	            o.pack0.xy  = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.pack0.zw  = TRANSFORM_TEX(v.texcoord,_BumpMap);
                
                float3 worldPos = mul(_Object2World, v.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;
                o.tSpace0 = fixed4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
                o.tSpace1 = fixed4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
                o.tSpace2 = fixed4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
	           
	            #if LIGHTMAP_ON
	            o.lightmap_uv.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif  
	            TRANSFER_SHADOW(o);
	            return o;
		    }
		    
		    half4 Lod1_frag (v2f_Lod1 i) : SV_Target
		    { 
	           
	            fixed4 c = 0;
	       
				fixed3  lightDir = _WorldSpaceLightPos0.xyz;
				fixed3  worldPos = fixed3(i.tSpace0.w, i.tSpace1.w, i.tSpace2.w);
				SurfaceOutput o;
				
				o.Normal =  H3DUnpackScaleNormal(tex2D (_BumpMap, i.pack0.zw),_BumpScale);
				o.Albedo =  tex2D( _MainTex , i.pack0.xy).rgb;
				
			    fixed3 worldN;
			    worldN.x = dot(i.tSpace0.xyz, o.Normal);
			    worldN.y = dot(i.tSpace1.xyz, o.Normal);
			    worldN.z = dot(i.tSpace2.xyz, o.Normal);
			    
			    UNITY_LIGHT_ATTENUATION(atten, i, worldPos)

			    o.Normal = worldN;
				
				c.rgb   += LightingH3DLambert(o, lightDir , atten);
				
				#if LIGHTMAP_ON
				c.rgb *= DecodeLightmap(H3D_SAMPLE_TEX2D(unity_Lightmap, i.lightmap_uv.xy ));
				#endif

				return c;
		    }


			ENDCG
			}//end pass
		} 
		
		SubShader {
		    LOD 200
		    
		    pass {
			Tags { "LightMode" = "ForwardBase" }
			
			CGPROGRAM
			#pragma multi_compile_fwdbase
	        #pragma vertex Lod2_vert
	        #pragma fragment Lod2_frag
	        
			struct v2f_Lod2 {
	            float4 pos         : SV_POSITION;
	            half2  pack0       : TEXCOORD0;
	            LIGHTING_COORDS(1,2)    
	            #if LIGHTMAP_ON
	            half2  lightmap_uv  : TEXCOORD3; 
	            #endif  
		    };
		    
		    #if LIGHTMAP_ON
		    H3D_LightMap_Declare 
			#endif  
			
			v2f_Lod2 Lod2_vert (appdata_full v)
		    {
	            v2f_Lod2 o;
	            UNITY_INITIALIZE_OUTPUT(v2f_Lod2,o);
	            o.pos =  mul (UNITY_MATRIX_MVP, v.vertex);
	            o.pack0  = TRANSFORM_TEX(v.texcoord,_MainTex);
	            #if LIGHTMAP_ON
	            o.lightmap_uv = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif  
	            TRANSFER_VERTEX_TO_FRAGMENT(o); 
	            return o;
		    }
		    
		    half4 Lod2_frag (v2f_Lod2 i) : SV_Target
		    { 
	           
	            fixed4 c = 0;
				c =  tex2D( _MainTex , i.pack0);
				fixed atten = LIGHT_ATTENUATION(i);
				
				#if LIGHTMAP_ON
				c.rgb *= DecodeLightmap(H3D_SAMPLE_TEX2D(unity_Lightmap, i.lightmap_uv ));
				#endif
				return c*atten;
		    }
			ENDCG
		}//end pass
	} 
		
		
    FallBack "H3D/InGame/SceneStaticObjects/SimpleDiffuse (Supports Lightmap)"
	}//end Category
}
