public class MathExt
{
    public static float Mod(float a, int b) 
    {
        return (a % b + b) % b;
    }
}