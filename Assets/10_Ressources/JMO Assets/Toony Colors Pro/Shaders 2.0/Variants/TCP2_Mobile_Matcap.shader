
// Toony Colors Pro+Mobile 2
// (c) 2014,2015 Jean Moreno


Shader "Hidden/Toony Colors Pro 2/Variants/Mobile Matcap"
{
	Properties
	{
		//TOONY COLORS
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
		_HColor ("Highlight Color", Color) = (0.6,0.6,0.6,1.0)
		_SColor ("Shadow Color", Color) = (0.2,0.2,0.2,1.0)
		
		//DIFFUSE
		_MainTex ("Main Texture (RGB) Spec/MatCap Mask (A) ", 2D) = "white" {}
		
		//TOONY COLORS RAMP
		_Ramp ("#RAMPT# Toon Ramp (RGB)", 2D) = "gray" {}
		_RampThreshold ("#RAMPF# Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("#RAMPF# Ramp Smoothing", Range(0.01,1)) = 0.1
		
		//BUMP
		_BumpMap ("#NORM# Normal map (RGB)", 2D) = "bump" {}
		
		//MATCAP
		_MatCap ("#MC# MatCap (RGB)", 2D) = "black" {}
		
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		
		#include "../Include/TCP2_Include.cginc"
		
		#pragma surface surf ToonyColors vertex:vert noforwardadd interpolateview halfasview
		#pragma glsl
		#pragma target 3.0
		
		#pragma shader_feature TCP2_RAMPTEXT
		#pragma shader_feature TCP2_BUMP
		#pragma shader_feature TCP2_MC TCP2_MCMASK
		#pragma shader_feature TCP2_LIGHTMAP
		
		//================================================================
		// VARIABLES
		
		fixed4 _Color;
		sampler2D _MainTex;
	#if TCP2_MC || TCP2_MCMASK
		sampler2D _MatCap;
	#endif
		
	#if TCP2_BUMP
		sampler2D _BumpMap;
	#endif
		
		struct Input
		{
			half2 uv_MainTex : TEXCOORD0;
	#if TCP2_BUMP
			half2 uv_BumpMap : TEXCOORD1;
	#endif
	#if TCP2_MC || TCP2_MCMASK
			half2 matcap;
	#endif
		};
		
		//================================================================
		// VERTEX FUNCTION
		
		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
	#if TCP2_MC || TCP2_MCMASK
			//MATCAP
			half2 capCoord;
			capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz,v.normal);
			capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz,v.normal);
			o.matcap = capCoord * 0.5 + 0.5;
	#endif
		}
		
		//================================================================
		// SURFACE FUNCTION
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 main = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = main.rgb * _Color.rgb;
			o.Alpha = main.a * _Color.a;
			
	#if TCP2_BUMP
			//Normal map
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	#endif
	#if TCP2_MCMASK
			o.Emission += tex2D(_MatCap, IN.matcap).rgb * main.a * _HColor.a;
	#elif TCP2_MC
			o.Emission += tex2D(_MatCap, IN.matcap).rgb;
	#endif
		}
		
		ENDCG
		
	}
	
	Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector"
}