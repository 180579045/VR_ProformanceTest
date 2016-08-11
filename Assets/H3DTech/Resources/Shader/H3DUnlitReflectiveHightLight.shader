// lod1 per pixel specular
// Lod2 vertexLit specular
// Lod3 no reflect

Shader "H3D/InGame/Character/Unlit/ReflectiveHighLight" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_Color("Main Color",Color) = (0.8,0.8,0.8,1)
		_ReflTex ("Reflect Texture", 2D) = "white" {}
		_ReflColor ("Reflect Color", Color) = (1.0,1.0,1.0,1.0)
		_ReflPower ("Reflect Power",Range(0.0,1.0)) = 0.5
		_SHLightingScale("LightProbe Effect",Range(0.0,2)) = 1
		
		_SpecColor ("Specular Color", Color) = (1, 1, 1, 1)
	    _Shininess ("Shininess", Range (0.01, 1)) = 0.078125	
	    _SpecOffset("Specular Offset from Camera", Vector) = (1, 10, 2, 0)
	    _SpecRange ("Specular Range", Float) = 20
	}
	
	Category {
        Tags { "Queue" = "Geometry" "IgnoreProjector" = "True" "RenderType"="Opaque" }
        
        CGINCLUDE
        #include "H3DFramework.cginc"
        sampler2D _MainTex;
	    sampler2D _ReflTex;
        half4     _MainTex_ST;
        fixed     _ReflPower;
        fixed3    _ReflColor;
        half3     _SpecOffset;
        half      _SpecRange;
        fixed3    _Color;
        ENDCG 
        
        SubShader {
		  LOD 400
		  pass{
				Tags { "LightMode" = "ForwardBase" }
				CGPROGRAM
		        #pragma vertex   Lod1_vert
		        #pragma fragment Lod1_frag
		        struct v2f_lod1 {
		            float4 pos : SV_POSITION;  
		            float2 muv : TEXCOORD0;
		            float2 uv  : TEXCOORD1;
		            H3D_AMBIENT_LIGHT_COODS(2) 
		            half3  viewPos : TEXCOORD3;
		            half3  viewNormal : TEXCOORD4; 
		        }; 
		        
		        v2f_lod1 Lod1_vert (appdata_base v)
		        {
		        	v2f_lod1 o;
		            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		            o.muv = TRANSFORM_TEX(v.texcoord, _MainTex);
		            o.uv = H3DObjSpaceSphereMapUV(v.vertex,v.normal); 
		            
		            o.viewPos = mul(UNITY_MATRIX_MV, v.vertex);
		            o.viewNormal = mul((float3x3)UNITY_MATRIX_MV, v.normal);
		            
		            H3D_AMBIENT_LIGHT(o);
		            
		            return o;
		        }
		        
		        half4 Lod1_frag (v2f_lod1 i) : SV_Target
		        { 
		            
		            float3 viewDir = float3(0,0,1);
		            float3 viewLightPos = _SpecOffset * float3(1,1,-1);
		            float3 dirToLight =   i.viewPos.xyz - viewLightPos;
		            float3 h = (viewDir + normalize(-dirToLight)) * 0.5;
		            float  atten = 1.0 - saturate(length(dirToLight) / _SpecRange);
		            fixed3 spec = _SpecColor * pow(saturate(dot(i.viewNormal, normalize(h))), _Shininess * 128) * 2 * atten;
		            
		            half4 mc = tex2D(_MainTex,i.muv);
		            half3 albedo = lerp(mc.xyz * _Color,tex2D(_ReflTex,i.uv).xyz*_ReflColor,_ReflPower);
		            fixed4 c = 0;
		            c.rgb  = albedo;
					c.rgb +=  H3D_AMBIENT_GETLIGHT(i) * albedo;
					c.rgb +=  spec * mc.a;
					
					c.a = mc.a;
					return c;
		   
		        }
		        ENDCG 
		    }  
		} 
        
		SubShader {
		  LOD 300
		  pass{
				Tags { "LightMode" = "ForwardBase" }
				CGPROGRAM
		        #pragma vertex   Lod2_vert
		        #pragma fragment Lod2_frag
		        struct v2f_lod2 {
		            float4 pos : SV_POSITION;  
		            float2 muv : TEXCOORD0;
		            float2 uv  : TEXCOORD1;
		            H3D_AMBIENT_LIGHT_COODS(2) 
		            fixed3 spec : TEXCOORD3; //Lod2
		        }; 
		      
		        //Lod2
		        v2f_lod2 Lod2_vert (appdata_base v)
		        {
		            
		            v2f_lod2 o;
		            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		            o.muv = TRANSFORM_TEX(v.texcoord, _MainTex);
		            o.uv = H3DObjSpaceSphereMapUV(v.vertex,v.normal); 
		            
		           
		            float3 viewDir = float3(0,0,1);
		            
		            float4 viewPos = mul(UNITY_MATRIX_MV, v.vertex);
		            float3 viewNormal = mul((float3x3)UNITY_MATRIX_MV, v.normal);
		            
		            float3 viewLightPos = _SpecOffset * float3(1,1,-1);
		            float3 dirToLight =   viewPos.xyz - viewLightPos;
		            float3 h = (viewDir + normalize(-dirToLight)) * 0.5;
		            float  atten = 1.0 - saturate(length(dirToLight) / _SpecRange);
		            o.spec = _SpecColor * pow(saturate(dot(viewNormal, normalize(h))), _Shininess * 128) * 2 * atten;
		            
		            H3D_AMBIENT_LIGHT(o);
		            return o;
		        }
		        half4 Lod2_frag (v2f_lod2 i) : SV_Target
		        { 
		            half4 mc = tex2D(_MainTex,i.muv);
		          
		            half3 albedo = lerp(mc.xyz,tex2D(_ReflTex,i.uv).xyz*_ReflColor,_ReflPower);
		            
		            fixed4 c = 0;
		            c.rgb = albedo;
					c.rgb +=  H3D_AMBIENT_GETLIGHT(i)*albedo;
					c.rgb +=  i.spec * mc.a;
					
					c.a = mc.a;
					return c;
		   
		        }
		        ENDCG 
		    }  
		} 
		SubShader {
		  LOD 200
		  pass{
				Tags { "LightMode" = "ForwardBase" }
				CGPROGRAM
		        #pragma vertex Lod3_vert
		        #pragma fragment Lod3_frag
		        struct v2f_lod3 {
		            float4 pos : SV_POSITION;  
		            float2 muv : TEXCOORD0;
		            H3D_AMBIENT_LIGHT_COODS(1) 
		        }; 
		        v2f_lod3 Lod3_vert (appdata_base v)
		        {
		            v2f_lod3 o;
		            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		            o.muv = TRANSFORM_TEX(v.texcoord, _MainTex);        
		            H3D_AMBIENT_LIGHT(o);
		            return o;
		        }
		        half4 Lod3_frag (v2f_lod3 i) : SV_Target
		        { 
		            half3 albedo = tex2D(_MainTex, i.muv).rgb * _Color;
		            fixed4 c = 0;
		            c.rgb  = albedo;
					c.rgb +=  H3D_AMBIENT_GETLIGHT(i)*albedo;
					return c;
		   
		        }
		        ENDCG 
		    }  
		} 
    FallBack "H3D/InGame/SceneDynamicObjects/SimpleDiffuse(VertexLit shader cast shadow)"
    }//end category
}
