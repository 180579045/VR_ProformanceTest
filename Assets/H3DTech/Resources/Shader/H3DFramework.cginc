// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'


#ifndef H3D_FIRMWORK_INCLUDED
#define H3D_FIRMWORK_INCLUDED
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"			
#include "HLSLSupport.cginc"
			
// lightmap 
#if UNITY_VERSION >= 500
	#define H3D_LightMap_Declare  	
#else
	#define H3D_LightMap_Declare  	fixed4 unity_LightmapST;   // sampler2D unity_Lightmap;
	
#endif


#if UNITY_VERSION >= 500
	#define H3D_SAMPLE_TEX2D(texture_map,input_uv) UNITY_SAMPLE_TEX2D(texture_map, input_uv)
#else
    #define H3D_SAMPLE_TEX2D(texture_map,input_uv) tex2D(texture_map,input_uv)
#endif
// end lightmap

// H3D shader variables

half _Shininess;

// H3D shader variables end

//lighting
inline fixed4 LightingH3DLambert (SurfaceOutput s, fixed3 lightDir, fixed atten)
{
	fixed diff = max (0, dot (s.Normal, lightDir));
	fixed4 c;
	c.rgb = s.Albedo *  _LightColor0.rgb * diff * atten;
	c.a = s.Alpha;
	return c;
}

inline fixed4 LightingH3DBlinnPhong(SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
{

	fixed3 h    = normalize(lightDir + viewDir);
	fixed  diff = max (0, dot (s.Normal, lightDir));
	fixed  nh   = max (0, dot (s.Normal, h));
	fixed  sp   = pow (nh, s.Specular*128.0) * s.Gloss;
	fixed4 c;	
	c.rgb       = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * sp) * atten;
	c.a = s.Alpha;
	return c;
}

inline fixed4 LightingH3DBlinnPhongHalf(SurfaceOutput s, fixed3 lightDir, fixed3 halfViewDir, fixed atten)
{


	fixed  diff = max (0, dot (s.Normal, lightDir));
	fixed  nh   = max (0, dot (s.Normal, halfViewDir));
	fixed  sp   = pow (nh, s.Specular*128.0) * s.Gloss;
	fixed4 c;	
	c.rgb       = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * sp) * atten;
	c.a = s.Alpha;
	return c;
}


// lightmap + lighting
inline fixed4 H3DBlinnPhongAdd(SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
{
	fixed3 h  = normalize(lightDir + viewDir);
	fixed  nh = max (0, dot (s.Normal, h));
	fixed  sp = pow (nh, s.Specular*128.0) * s.Gloss;
	fixed4 c;	
	c.rgb     = (s.Albedo + _LightColor0.rgb * _SpecColor.rgb * sp ) * atten;
	c.a       = s.Alpha;
	return c;
}

inline fixed4 H3DBlinnPhongAddHalf(SurfaceOutput s,fixed3 halfView,fixed atten)
{
   fixed nh = max (0, dot (s.Normal, halfView));
   fixed spec = pow (nh, s.Specular*128) * s.Gloss;
   fixed4 c;
   c.rgb = (s.Albedo + _LightColor0.rgb * spec * _SpecColor.rgb) * atten;
   c.a = s.Alpha;
   return c;
}
// lightmap + lighting end


//vertex lit lighting
inline fixed3 H3DLightingLambert (half3 normal, half3 lightDir)
{
	fixed diff = max (0, dot (normal, lightDir));		
	return _LightColor0.rgb * diff ;
}

inline fixed3 H3DLightingBPhong (half3 normal,half3 viewDir, half3 lightDir)
{
    half3 h = normalize (lightDir + viewDir);
	fixed diff = max (0, dot (normal, lightDir));
	half nh = max (0, dot (normal,h));
	half spec = pow (nh, _Shininess*128.0)*4.0;
	return  _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor * spec;
}

#define H3D_LIGHT_COODS(indx1) fixed3 _vlight : TEXCOORD##indx1;
#define H3D_LAMBERT_LIGHT(o) \
	float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;\
	fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);\
	o._vlight = ShadeSH9 (float4(worldNormal,1.0));\
	o._vlight += H3DLightingLambert (worldNormal, _WorldSpaceLightPos0.xyz);\
	o._vlight += Shade4PointLights (\
    unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,\
    unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,\
    unity_4LightAtten0, worldPos, worldNormal)
    
#define H3D_BLINNPHONG_LIGHT(o) \
	float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;\
	fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);\
	fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));\
	o._vlight = ShadeSH9 (float4(worldNormal,1.0));\
	o._vlight += H3DLightingBPhong (worldNormal,worldViewDir, _WorldSpaceLightPos0.xyz);\
	o._vlight += Shade4PointLights (\
    unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,\
    unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,\
    unity_4LightAtten0, worldPos, worldNormal)
    
#define H3D_SIMPLE_GETLIGHT(i) i._vlight
#define H3D_SURFACE_LIGHT(i,o)  i._vlight * o.Albedo

half      _SHLightingScale;
#define H3D_AMBIENT_LIGHT_COODS(indx1) fixed3 _ambientLight : TEXCOORD##indx1;
#define H3D_AMBIENT_LIGHT(o)\
	fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);\
	o._ambientLight = ShadeSH9 (float4(worldNormal,1.0)) * _SHLightingScale;
#define H3D_AMBIENT_GETLIGHT(i) i._ambientLight
//vertex lit lighting end


//lighting end
    
//TexGen SphereMap
float2 H3DObjSpaceSphereMapUV(float3 refl)
{

	refl = mul((float3x3)UNITY_MATRIX_MV, refl);
	refl.z += 1;
	float m = 2 * length(refl);
	return refl.xy / m + 0.5;
}

float2 H3DObjSpaceSphereMapUV(float4 vertex,float3 normal) 
{
	 float3 viewDir = normalize(ObjSpaceViewDir(vertex));      
	 float3 r = reflect(-viewDir, normal);
	 return H3DObjSpaceSphereMapUV(r);
 }
//TexGen SphereMap end

half3 H3DUnpackScaleNormal(half4 packednormal, half bumpScale)
{
    #if defined(UNITY_NO_DXT5nm)
        half3 normal;
		normal.xy = (packednormal.xy * 2 - 1) * bumpScale; 
		normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
		return normal;
    #else
		half3 normal;
		normal.xy = (packednormal.wy * 2 - 1);
		#if (SHADER_TARGET >= 30)
			// SM2.0: instruction count limitation
			// SM2.0: normal scaler is not supported
			normal.xy *= bumpScale;
		#endif
		normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
		return normal;
    #endif
}	
 
 



#endif //H3D_FIRMWORK_INCLUDED
