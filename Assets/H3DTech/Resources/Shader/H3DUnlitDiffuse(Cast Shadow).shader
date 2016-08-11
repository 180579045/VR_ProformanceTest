// it is same with H3DUnlitDiffuse
Shader "H3D/InGame/Character/Unlit/Diffuse" {
    Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_Color("Main Color" ,color) = (0.5,0.5,0.5,1.0)
		_SHLightingScale("Ambient LightProbe Effect",Range(0.0,10.0)) = 1.0
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
            fixed4 mc = tex2D(_MainTex, i.muv);
            fixed4 c = 0;
			c.rgb  = mc.rgb * _Color;
			c.a    = mc.a;
			c.rgb += c.rgb * H3D_AMBIENT_GETLIGHT(i);
			
			return c;
   
        }
        ENDCG 
        

		SubShader {
		  LOD 200
		  pass{
		        Name "SHLighting"
				Tags { "LightMode" = "ForwardBase" }
				CGPROGRAM
		        #pragma vertex Lod3_vert
		        #pragma fragment Lod3_frag
		        ENDCG 
		    }  
		} 
        FallBack "H3D/InGame/SceneDynamicObjects/SimpleDiffuse(VertexLit shader cast shadow)"
    }//end category
}
