namespace Code;

public interface ICoder
{
    BitMessage Encode(BitMessage message);

    BitMessage Decode(BitMessage message);
}