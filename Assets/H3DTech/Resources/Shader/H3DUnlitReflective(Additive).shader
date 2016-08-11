Shader "H3D/InGame/SceneDynamicObjects/Unlit/Reflective" {
	Properties 
	{
		_MainTex ("主纹理(RGB)", 2D) = "white" {} 
		_TintColor("主色调",Color) = (1.0,1.0,1.0,1.0)
		_ReflTex ("反射纹理", 2D) = "white" {}
		_ReflPowerTex("反射率纹理(A)",2D) = "white"{}
		_ReflColor ("反射颜色", Color) = (1.0,1.0,1.0,1.0)
		_ReflPower ("反射强度",Range(0.0,1.0)) = 1.0
		_ReflAnimFactor("反射纹理坐标动画权重",Range(0.0,1.0)) = 0.0
		_ReflTexUOffsetSpeed ("反射偏移水平速度",float) = 0
		_ReflTexVOffsetSpeed ("反射偏移纵向速度",float) = 0
		_SHLightingScale("Ambient LightProbe Effect",Range(0.0,10.0)) = 1
	}
Category {
    Tags { "Queue" = "Geometry" "IgnoreProjector" = "True"  "RenderType"="Opaque" }
    CGINCLUDE
        #include "H3DFramework.cginc"
        sampler2D _MainTex;
		sampler2D _ReflTex;
		sampler2D _ReflPowerTex;
		half4     _MainTex_ST;
		half4     _ReflColor; 
		half      _ReflPower;
		half      _ReflAnimFactor;
		half      _ReflTexUOffsetSpeed;
		half      _ReflTexVOffsetSpeed;	
		fixed4    _TintColor;
		
		
    ENDCG 
	SubShader {
		LOD 300
		Pass {  
		        Tags { "LightMode" = "ForwardBase" }
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				struct v2f {
					float4 vertex    : SV_POSITION;
					half2  uv        : TEXCOORD0;
					half2  ruv       : TEXCOORD1;
					H3D_AMBIENT_LIGHT_COODS(2)
				};
				v2f vert (appdata_base v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex); 
					o.ruv = H3DObjSpaceSphereMapUV(v.vertex,v.normal);
		            H3D_AMBIENT_LIGHT(o);
					return o;
				}
				fixed4 frag (v2f i) : SV_Target
				{
					float realTime = _Time.y;
					fixed4 mc = tex2D(_MainTex, i.uv);
					half2 reflUVOff = half2(
						fmod( realTime*_ReflTexUOffsetSpeed * _ReflAnimFactor , 1.0) , 
						fmod( realTime*_ReflTexVOffsetSpeed * _ReflAnimFactor , 1.0)
					);
					fixed4 refColor = tex2D(_ReflTex, i.ruv + reflUVOff  );
					fixed4 refPowerColor = tex2D(_ReflPowerTex, i.uv);
					fixed4 c = 0;
					
					c.rgb    = lerp(mc.rgb*_TintColor,refColor.rgb*_ReflColor ,_ReflPower*mc.a*refPowerColor.a);
					c.rgb   += c.rgb * H3D_AMBIENT_GETLIGHT(i) ;
					return c;
				}
				ENDCG
		}
	} 
	
	
	SubShader 
	{
		LOD  200
		Pass {  
		        Tags { "LightMode" = "ForwardBase" }
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				struct v2f {
					float4 vertex    : SV_POSITION;
					half2  uv        : TEXCOORD0;
					H3D_AMBIENT_LIGHT_COODS(2)
				};
				v2f vert (appdata_base v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex); 
		            H3D_AMBIENT_LIGHT(o);
					return o;
				}
				fixed4 frag (v2f i) : SV_Target
				{
					fixed4 mc = tex2D(_MainTex, i.uv);
					fixed4 c  = 0;
					c.rgb     = mc.rgb * _TintColor;
					c.rgb    += c.rgb * H3D_AMBIENT_GETLIGHT(i);
					return c;
				}
				ENDCG
		}
	}   
	
	 FallBack "H3D/InGame/SceneDynamicObjects/SimpleDiffuse(VertexLit shader cast shadow)"
  }
}


