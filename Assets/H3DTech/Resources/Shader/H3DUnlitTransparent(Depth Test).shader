// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "H3D/InGame/SceneDynamicObjects/Unlit/Transparent/Diffuse(depath test)"  {
Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_Color("Main Color",color) =(0.8,0.8,0.8,1)
		_SHLightingScale("Ambient LightProbe Effect",Range(0.0,10.0)) = 2
		_Alpha("Alpha", Range(0,1)) = 1	
}
Category {
    Tags { "Queue"="Transparent" "IgnoreProjector" = "True"  }
	SubShader {
	  LOD 200
	  Blend SrcAlpha OneMinusSrcAlpha 
	  Pass {
        ZWrite On
        ColorMask 0
      }
	  pass{
	     
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
	        #pragma vertex Lod3_vert
	        #pragma fragment Lod3_frag
	        #include "H3DFramework.cginc"
		    struct v2f {
	            float4 pos : SV_POSITION;
	            half2 muv : TEXCOORD0;
	            H3D_AMBIENT_LIGHT_COODS(2)
	        };
	        
	        sampler2D _MainTex;
	        half4     _MainTex_ST;
	        fixed3    _Color;
	        fixed     _Alpha;
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
				c.rgb += H3D_AMBIENT_GETLIGHT(i) * c.rgb;
				c.a   = _Alpha * mc.a;
				return c;
	   
	        }
	        ENDCG 
	    }  
	} 
    FallBack "H3D/InGame/SceneDynamicObjects/SimpleDiffuse(VertexLit shader cast shadow)"
}//end category
}
