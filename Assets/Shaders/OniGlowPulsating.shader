Shader "Custom/Oni_Lantern_Blazing"
{
    Properties
    {
        // Основная текстура
        [MainTexture] _MainTex("Base Texture (Sprite)", 2D) = "white" {}
        
        // Текстура-маска свечения
        _GlowTex("Glow Mask", 2D) = "black" {}
        
        // Базовый цвет (тлеющий уголек) - теперь более насыщенный
        [HDR] _MinColor("Min Glow Color", Color) = (1.5, 0.5, 0.0, 1.0)
        
        // Цвет пика (яркая вспышка пламени)
        [HDR] _MaxColor("Max Glow Color", Color) = (4.0, 1.5, 0.2, 1.0)
        
        // Скорость дыхания (3.0 = примерно 2 секунды на полный цикл)
        _PulseSpeed("Pulse Speed", Float) = 3.0
        
        // НОВЫЙ ПАРАМЕТР: Общий множитель яркости (выкручивай, если мало света)
        _GlowMultiplier("Global Brightness Multiplier", Range(1.0, 10.0)) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; 
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_GlowTex);
            SAMPLER(sampler_GlowTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MinColor;
                float4 _MaxColor;
                float _PulseSpeed;
                float _GlowMultiplier; // Подключаем нашу переменную в буфер
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                // 1. Основной спрайт
                float4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.color;
                
                // 2. Маска (где именно должно гореть)
                float4 glowMask = SAMPLE_TEXTURE2D(_GlowTex, sampler_GlowTex, input.uv);

                // 3. Волна от 0 до 1
                float sineWave = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;
                
                // 4. Плавно переходим от минимума к максимуму
                float4 currentColor = lerp(_MinColor, _MaxColor, sineWave);

                // 5. Умножаем цвет на наш новый РАЗГОНОЧНЫЙ МНОЖИТЕЛЬ
                currentColor *= _GlowMultiplier;

                // 6. Финальная сборка: накладываем раскаленный цвет на маску и плюсуем к фону
                float3 finalRGB = baseColor.rgb + (glowMask.rgb * currentColor.rgb);
                
                return float4(finalRGB, baseColor.a);
            }
            ENDHLSL
        }
    }
}