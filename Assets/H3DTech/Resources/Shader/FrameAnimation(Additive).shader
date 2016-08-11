﻿Shader "H3D/InGame/Particles/FrameAnimShader(Additive)" 
{
	Properties 
	{
		_MainTex ("动画纹理", 2D) = "white" {} 
		_HorizonTileNum("横向精灵数量",Float) = 8
		_VerticalTileNum("纵向精灵数量",Float) = 8
		_PlaySpeed("播放速度(秒/帧)",Float) = 2
		_CurrFrame("播放方式( <0 循环 >0 播放帧号 )",Float) = -1
		_Alpha("透明度",Range(0,1)) = 1
	}

	SubShader 
	{
	    LOD 200
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "ForceNoShadowCasting" = "True" "IgnoreProjector" = "True" } 

		Lighting Off
		Blend SrcAlpha One
		ZTest LEqual
		ZWrite Off
		Cull Off
		 
		Pass
		{
			Name "AnimPass"

			CGPROGRAM 
			#pragma vertex vert
			#pragma fragment frag  
			//#pragma alpha:blend
			
			sampler2D _MainTex; 

			half _HorizonTileNum;
			half _VerticalTileNum;
			half _PlaySpeed;
			half _CurrFrame;
			fixed _Alpha;
			
			struct VIN
			{
				half4 vertex : POSITION;
				half4 texcoord0 : TEXCOORD0;
				fixed4 vertcolor : COLOR;
			};

			struct PIN
			{
				half4 position : SV_POSITION;
				half4 texcoord0 : TEXCOORD0;
				fixed4 color : COLOR;
			};
			 
			PIN vert(VIN vin )
			{
				PIN o;
				half hNum = floor(_HorizonTileNum);
				half vNum = floor(_VerticalTileNum);
				half speed = floor(_PlaySpeed);

				half invHNum = 1/hNum;
				half invVNum = 1/vNum;

				half totalNum = hNum * vNum;
				
				half indx = speed * _Time.y;
				if( _CurrFrame >= 0 )
				{
					indx = _CurrFrame;
				}

				half currIndx = floor( fmod( indx , totalNum ) );
				  
				half hOffsetIndx = fmod( currIndx , hNum );
				half vOffsetIndx = floor( currIndx * invHNum );
				
				vOffsetIndx = vNum - vOffsetIndx - 1;

				half2 offset = half2(hOffsetIndx * invHNum,vOffsetIndx * invVNum );
				
				half4 finalTexcoord = vin.texcoord0;
				finalTexcoord.x *= invHNum;
				finalTexcoord.y *= invVNum;
				finalTexcoord.xy += offset;				 
				
				o.position = mul (UNITY_MATRIX_MVP, vin.vertex );
				o.texcoord0 = finalTexcoord;
				o.color = vin.vertcolor;
				return o;
			}

			fixed4 frag( PIN pin ) : COLOR 
			{  
				fixed4 finalColor = tex2D(_MainTex, pin.texcoord0.xy) * pin.color; 
				finalColor.a *= _Alpha;
				return finalColor; 
			}

			ENDCG

		}
 
	} 
	FallBack "H3D/InGame/SceneStaticObjects/SimpleDiffuse ( no Supports Lightmap)"
}
