Shader "H3D/InGame/Character/Unlit/Reflective(Two Side)" {
	Properties {
		_MainTex ("主纹理", 2D) = "white" {}
		_Color("主颜色", Color) = (0.8,0.8,0.8,1.0)
		_ReflTex ("反射纹理", 2D) = "white" {}
		_ReflColor ("反射颜色", Color) = (1.0,1.0,1.0,1.0)
		_ReflPower ("反射强度",Range(0.0,1.0)) = 0.5
		_SHLightingScale("Ambient LightProbe Effect",Range(0.0,10.0)) = 1
		
	}
	
	Category {
        Tags { "Queue" = "Geometry" "IgnoreProjector" = "True"  "RenderType"="Opaque"}
        cull off
        CGINCLUDE
        #include "H3DFramework.cginc"

        sampler2D _MainTex;
	    sampler2D _ReflTex;
        float4    _MainTex_ST;
        float     _ReflPower;
        half4     _ReflColor;
        fixed3    _Color;
     
        ENDCG 
        
		SubShader {
		  LOD 300
		  pass{
				Tags { "LightMode" = "ForwardBase" }
				cull off
				CGPROGRAM
		        #pragma vertex Lod2_vert
		        #pragma fragment Lod2_frag
		        
		        struct v2f {
		            float4 pos : SV_POSITION;
		            float2 muv : TEXCOORD0;
		            float2 uv  : TEXCOORD1;
		            H3D_AMBIENT_LIGHT_COODS(2) 
        		};
        
		        v2f Lod2_vert (appdata_base v)
		        {
		            v2f o;
		            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		            o.muv = TRANSFORM_TEX(v.texcoord, _MainTex);
		            
		            float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));	         
		            float3 r = reflect(-viewDir, v.normal);
		            o.uv = H3DObjSpaceSphereMapUV(r);
		            
		            H3D_AMBIENT_LIGHT(o);
		            return o;
		        }
		        half4 Lod2_frag (v2f i) : SV_Target
		        { 
		            fixed3 albedo = lerp(tex2D(_MainTex , i.muv).rgb * _Color,tex2D(_ReflTex,i.uv).rgb*_ReflColor,_ReflPower);
		            fixed4 c = 0;
		            c.rgb  = albedo;
					c.rgb  +=  H3D_AMBIENT_GETLIGHT(i) * albedo;
					return c;
		   
		        }
		        ENDCG 
		    }  
		} 
		SubShader {
		  LOD 200
		  pass{
				Tags { "LightMode" = "ForwardBase" }
				cull off
				CGPROGRAM
		        #pragma vertex Lod3_vert
		        #pragma fragment Lod3_frag
		        struct v2f {
		            float4 pos : SV_POSITION;
		            half2 muv : TEXCOORD0;
		            H3D_AMBIENT_LIGHT_COODS(1)
        		};
		        v2f Lod3_vert (appdata_base v)
		        {
		            v2f o;
		            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		            o.muv = TRANSFORM_TEX(v.texcoord, _MainTex);        
		            H3D_AMBIENT_LIGHT(o);
		            return o;
		        }
		        half4 Lod3_frag (v2f i) : SV_Target
		        { 
		            fixed3 albedo = tex2D(_MainTex, i.muv).rgb * _Color;
		            fixed4 c = 0;
		            c.rgb  = albedo;
					c.rgb +=  H3D_AMBIENT_GETLIGHT(i) * albedo;
					return c;
		        }
		        ENDCG 
		    }  
		} 
    FallBack "H3D/InGame/SceneDynamicObjects/SimpleDiffuse(VertexLit shader cast shadow)"
    }//end category
}
