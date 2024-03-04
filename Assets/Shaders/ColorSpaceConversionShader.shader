Shader "Unlit/ColorSpaceConversionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Difficulty = ("Difficulty", Range(0, 100)) = 50

        _DimensionState ("Dimension State", Int) = 0 // 0 = xPositive, 1 = xNegative, 2 = yPositive, 3 = yNegative. Default is 0(xPositive)
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
            
            // Step value for difficulty
            float defaultDifficultyStepValueDKLx = 2.55; // Default difficulty step value for DKLx
            float defaultDifficultyStepValueDKLy = 4; // Default difficulty step value for DKLy

            // DKLx, DKLy의 상한, 하한 값
            float DKLxUpperLimit = 255; // DKLx의 상한 값
            float DKLxBottomLimit = 0; // DKLx의 하한 값
            float DKLyUpperLimit = 200; // DKLy의 상한 값
            float DKLyBottomLimit = -200; // DKLy의 하한 값

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

                float3 adjustColor; // 최종 결과 값
                float defaultDifficultyStepValue; // 난이도 한 단계 당 기본 값

                float3 _InputRGB = float3(col.r*255, col.g*255, col.b*255);

                // RGB to DKL Cartesian Matrix by PsycoPy
                float3x3 RGB2DKLCartbyPsycoPy = float3x3(
                    0.25145542, 0.64933633, 0.09920825,
                    0.78737943, -0.55586618, -0.23151325,
                    0.26562825, 0.63933074, -0.90495899
                );

                // DKL Cartesian to RGB Matrix by PsycoPy
                float3x3 DKLCart2RGBbyPsychoPy = float3x3(
                    1.0, 0.999999998, -0.146199995f,
                    1.0, -0.389999999, 0.209400005,
                    1.0, 0.018, -0.999999998
                );
                
                // Convert RGB to DKL Cartesian
                float3 DKLCart = mul(RGB2DKLCartbyPsycoPy, _InputRGB);

                // DimensionState에 따라 색에 난이도 설정.
                if(_DimensionState == 0) // xPositive
                {
                    float defaultDifficultyValue = abs(DKLxUpperLimit - _InputRGB.x) / 2;
                    float difficultyStepValue = defaultDifficultyStepValueDKLx; // 난이도 한 단계 당 기본 2.55씩 늘어남.
                    if (_Difficulty > 50)
                    {
                        // X+의 Upper Limit에서 현재 Reference Color X 값 뺀 구간을 50으로 나눔 = 1%. Ref Color와 가까워지는 방향이면 1%씩 움직여서 ref color를 넘어가는 일이 없도록 함.
                        difficultyStepValue = defaultDifficultyStepValue / 50;
                    }

                    // 난이도가 반영된 차이값.
                    float difficultyAppliedValue = defaultDifficultyValue - (difficultyStepValue * (_Difficulty-50));

                    adjustColor = float3(
                        DKLCart.x + difficultyAppliedValue,
                        DKLCart.y,
                        DKLCart.z
                    );
                }
                else if(_DimensionState == 1) // xNegative
                {
                    float defaultDifficultyValue = abs(DKLxBottomLimit - _InputRGB.x) / 2;
                    float difficultyStepValue = defaultDifficultyStepValueDKLx; // 난이도 한 단계 당 기본 2.55씩 늘어남.
                    if (_Difficulty > 50)
                    {
                        // X-의 Bottom Limit에서 현재 Reference Color X 값 뺀 구간을 50으로 나눔 = 1%. Ref Color와 가까워지는 방향이면 1%씩 움직여서 ref color를 넘어가는 일이 없도록 함.
                        difficultyStepValue = defaultDifficultyStepValue / 50;
                    }

                    // 난이도가 반영된 차이값.
                    float difficultyAppliedValue = defaultDifficultyValue - (difficultyStepValue * (_Difficulty-50));

                    adjustColor = float3(
                        DKLCart.x - difficultyAppliedValue,
                        DKLCart.y,
                        DKLCart.z
                    );
                }
                
                else if(_DimensionState == 2) // yPositive
                {
                    float defaultDifficultyValue = abs(DKLyUpperLimit - _InputRGB.y) / 2;
                    float difficultyStepValue = defaultDifficultyStepValueDKLy; // 난이도 한 단계 당 기본 4씩 늘어남.
                    if (_Difficulty > 50)
                    {
                        // Y+의 Upper Limit에서 현재 Reference Color Y 값 뺀 구간을 50으로 나눔 = 1%. Ref Color와 가까워지는 방향이면 1%씩 움직여서 ref color를 넘어가는 일이 없도록 함.
                        difficultyStepValue = defaultDifficultyStepValue / 50;
                    }

                    // 난이도가 반영된 차이값.
                    float difficultyAppliedValue = defaultDifficultyValue - (difficultyStepValue * (_Difficulty-50));

                    adjustColor = float3(
                        DKLCart.x,
                        DKLCart.y + difficultyAppliedValue,
                        DKLCart.z
                    );
                }
                
                else if(_DimensionState == 3) // yNegative
                {
                    float defaultDifficultyValue = abs(DKLyBottomLimit - _InputRGB.y) / 2;
                    float difficultyStepValue = defaultDifficultyStepValueDKLy; // 난이도 한 단계 당 기본 4씩 늘어남.
                    if (_Difficulty > 50)
                    {
                        // Y-의 Bottom Limit에서 현재 Reference Color Y 값 뺀 구간을 50으로 나눔 = 1%. Ref Color와 가까워지는 방향이면 1%씩 움직여서 ref color를 넘어가는 일이 없도록 함.
                        difficultyStepValue = defaultDifficultyStepValue / 50;
                    }

                    // 난이도가 반영된 차이값.
                    float difficultyAppliedValue = defaultDifficultyValue - (difficultyStepValue * (_Difficulty-50));

                    adjustColor = float3(
                        DKLCart.x,
                        DKLCart.y - difficultyAppliedValue,
                        DKLCart.z
                    );
                }
                
                return fixed4(adjustColor, 1.0);
            }
            ENDCG
        }
    }
}
