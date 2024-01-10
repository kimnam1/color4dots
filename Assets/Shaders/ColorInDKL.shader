Shader "Unlit/ColorInDKL"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		_LMSAdjust("LMS Adjust", Vector) = (1, 1, 1)
		_DKLAdjust("DKL Adjust", Vector) = (1, 1, 1)
        _Red("Red in RGB(0~255)", Range(0, 255)) = 0
        _Green("Green in RGB(0~255)", Range(0, 255)) = 0
        _Blue("Blue in RGB(0~255)", Range(0, 255)) = 0

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			float3 _LMSAdjust;
			float3 _DKLAdjust;
            float _Red;
            float _Green;
            float _Blue;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                float3 _InputRGB = float3(_Red/255, _Green/255, _Blue/255);
	
				// RGB to XYZ matrix
				float3x3 rgb2xyz = float3x3(0.4124564, 0.3575761, 0.1804375, 0.2126729, 0.7151522, 0.0721750, 0.0193339, 0.1191920, 0.9503041);

				// XYZ to RGB matrix
				float3x3 xyz2rgb = float3x3(3.240455, -1.537139, -0.4985315, -0.969266, 1.876011, 0.041556, 0.055643, -0.204026, 1.057225);

				// XYZ to LMS matrix(Bradford style)
				float3x3 xyz2lms = float3x3(0.8951, 0.2664, -0.1614, -0.7502, 1.7135, 0.0367, 0.0389, -0.0685, 1.0296);

				// LMS to XYZ matrix(Bradford style)
				float3x3 lms2xyz = float3x3(0.986992905, -0.147054256, 0.159962652, 0.43230527, 0.518360272, 0.049291228, -0.008528665, 0.040042822, 0.968486696);

				// LMS to DKL
				float3x3 lms2dkl = float3x3(1, -1, 0, -1, -1, 1, 1, 1, 0);

				// DKL to LMS
				float3x3 dkl2lms = float3x3(0.5, 0, 0.5, -0.5, 0, 0.5, 0, 1, 1);
				
				// Source RGB to XYZ
				float3 XYZColor = mul(rgb2xyz, _InputRGB);
                // XYZ to LMS
				float3 LMSColor = mul(xyz2lms, XYZColor);

                //LMS Adjust
				LMSColor = float3(_LMSAdjust.x * LMSColor.x, _LMSAdjust.y * LMSColor.y, _LMSAdjust.z * LMSColor.z);

				// LMS to DKL
				float3 DKLColor = mul(lms2dkl, LMSColor);

				// DKL Adjust
				DKLColor = float3(_DKLAdjust.x * DKLColor.x, _DKLAdjust.y * DKLColor.y, _DKLAdjust.z * DKLColor.z);

                // DKL to LMS
			    LMSColor = mul(dkl2lms, DKLColor);

                // LMS to RGB return
				float3 xyz = mul(lms2xyz, LMSColor);
				float3 rgb = mul(xyz2rgb, xyz);


                return fixed4(rgb, 1.0);
            }
            ENDCG
        }
    }
}
