namespace ShareInvest;

public class Normalization
{
    public dynamic Denormalize(int arg) => arg * denominator + min;

    public dynamic Denormalize(uint arg) => arg * denominator + min;

    public dynamic Denormalize(long arg) => arg * denominator + min;

    public dynamic Denormalize(ulong arg) => arg * denominator + min;

    public dynamic Denormalize(double arg) => arg * denominator + min;

    public dynamic Denormalize(float arg) => arg * denominator + min;

    public dynamic Denormalize(decimal arg) => arg * denominator + min;

    public dynamic Normalize(int arg) => (arg - min) / denominator;

    public dynamic Normalize(uint arg) => (arg - min) / denominator;

    public dynamic Normalize(long arg) => (arg - min) / denominator;

    public dynamic Normalize(ulong arg) => (arg - min) / denominator;

    public dynamic Normalize(double arg) => (arg - min) / denominator;

    public dynamic Normalize(float arg) => (arg - min) / denominator;

    public dynamic Normalize(decimal arg) => (arg - min) / denominator;

    public Normalization(dynamic max, dynamic min)
    {
        denominator = max - min;

        this.min = min;
    }
    readonly dynamic min;
    readonly dynamic denominator;
}