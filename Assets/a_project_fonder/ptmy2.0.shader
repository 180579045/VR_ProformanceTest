Shader "grb/ptmy2.0" {
	Properties {
		
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

	}
	SubShader {
		Tags { "Queue"="Transparent" }
		GrabPass{}
		Pass{
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _GrabTexture;
			half4     _GrabTexture_ST;
			half4     _MainTex_ST;
			struct v2f {
				float4 pos:POSITION;
				half4  uv :TEXCOORD0;
				half2 muv :TEXCOORD1;
			};
			
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
//				o.uv  = TRANSFORM_TEX(v.texcoord,_GrabTexture);
				o.muv  = TRANSFORM_TEX(v.texcoord,_MainTex);
				half4 screenUV = ComputeGrabScreenPos(o.pos);
				o.uv.xy = screenUV.xy/screenUV.w;
//				o.uv =v.texcoord;
				return o;
			}
			fixed4 frag(v2f i):COLOR
			{
//				fixed4 c = tex2D(_GrabTexture,half2(1-i.uv.x,1-i.uv.y));
				fixed4 mc = tex2D(_MainTex,i.muv);
				fixed4 c  = tex2Dproj(_GrabTexture,UNITY_PROJ_COORD(i.uv));
//				half last_x = i.uv.x/i.uv.w;
//				half last_y = i.uv.y/i.uv.w;
//				fixed4 texcol = tex2D(_GrabTexture,half2(i.uv.xy));
				return mc+c;
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
