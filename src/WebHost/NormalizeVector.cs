namespace WebHost;

public class NormalizeVector
{
    public static float[] Handle(float[] vector)
    {
        double sumSquares = 0;
        foreach (var v in vector)
            sumSquares += v * v;

        double norm = Math.Sqrt(sumSquares);
        if (norm == 0) return vector;

        float[] normalized = new float[vector.Length];
        for (int i = 0; i < vector.Length; i++)
            normalized[i] = (float)(vector[i] / norm);

        return normalized;
    }
}
