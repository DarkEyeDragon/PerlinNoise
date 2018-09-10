using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexNoise
{
    class PerlinNoise
    {

        public float[] Output { get; set; }
        public void Generate1D(int nCount, float[] fSeed, int nOctaves)
        {
            float[] output = new float[nCount];
            for (int x = 0; x < nCount; x++)
            {
                float fNoise = 0.0f;
                float fScale = 1.0f;
                float fScaleAcc = 0.0f;
                for (int o = 0; o < nOctaves; o++)
                {
                    int nPitch = nCount >> o;
                    int nSample1 = (x / nPitch) * nPitch;
                    int nSample2 = (nSample1 + nPitch) % nCount;

                    float fBlend = (float)(x - nSample1) / (float)nPitch;
                    float fSample = (1.0f - fBlend) * fSeed[nSample1] + fBlend * fSeed[nSample2];
                    fNoise += fSample * fScale;
                    fScaleAcc += fScale;
                    fScale = fScale / 2.0f;
                }

                output[x] = fNoise / fScaleAcc;
            }

            Output = output;
        }

        public void Generate2D(int nWidth, int nHeight, float[] fSeed, int nOctaves, float fBias)
        {
            float[] output = new float[nWidth*nHeight];
            for (int x = 0; x < nWidth; x++)
                for (int y = 0; y < nHeight; y++)
                {
                    float fNoise = 0.0f;
                    float fScaleAcc = 0.0f;
                    float fScale = 1.0f;

                    for (int o = 0; o < nOctaves; o++)
                    {
                        int nPitch = nWidth >> o;
                        int nSampleX1 = (x / nPitch) * nPitch;
                        int nSampleY1 = (y / nPitch) * nPitch;

                        int nSampleX2 = (nSampleX1 + nPitch) % nWidth;
                        int nSampleY2 = (nSampleY1 + nPitch) % nWidth;

                        float fBlendX = (float)(x - nSampleX1) / (float)nPitch;
                        float fBlendY = (float)(y - nSampleY1) / (float)nPitch;

                        float fSampleT = (1.0f - fBlendX) * fSeed[nSampleY1 * nWidth + nSampleX1] + fBlendX * fSeed[nSampleY1 * nWidth + nSampleX2];
                        float fSampleB = (1.0f - fBlendX) * fSeed[nSampleY2 * nWidth + nSampleX1] + fBlendX * fSeed[nSampleY2 * nWidth + nSampleX2];

                        fScaleAcc += fScale;
                        fNoise += (fBlendY * (fSampleB - fSampleT) + fSampleT) * fScale;
                        fScale = fScale / fBias;
                    }

                    // Scale to seed range
                    output[y * nWidth + x] = fNoise / fScaleAcc;
                }

            Output = output;
        }
    }
}
