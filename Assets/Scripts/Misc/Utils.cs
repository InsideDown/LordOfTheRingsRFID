using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : Singleton<Utils> {

    protected Utils() {}

    public Vector2 RGBToXY(int r, int g, int b)
    {
        float cR = r / 255;
        float cG = g / 255;
        float cB = b / 255;

        float red = (cR > 0.04045) ? Mathf.Pow((cR + 0.055f) / (1.0f + 0.055f), 2.4f) : (cR / 12.92f);
        float green = (cG > 0.04045) ? Mathf.Pow((cG + 0.055f) / (1.0f + 0.055f), 2.4f) : (cG / 12.92f);
        float blue = (cB > 0.04045) ? Mathf.Pow((cB + 0.055f) / (1.0f + 0.055f), 2.4f) : (cB / 12.92f);

        float X = red * 0.664511f + green * 0.154324f + blue * 0.162028f;
        float Y = red * 0.283881f + green * 0.668433f + blue * 0.047685f;
        float Z = red * 0.000088f + green * 0.072310f + blue * 0.986039f;

        float finalX = X / (X + Y + Z);
        float finalY = Y / (X + Y + Z);

        return new Vector2(finalX, finalY);
    }
}
