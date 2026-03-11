using System;

public readonly struct StateKey : IEquatable<StateKey>
{
    public Type StateType { get; }
    public string VariationLabel { get; }

    public StateKey(Type stateType, string variationLabel = "")
    {
        StateType = stateType;
        VariationLabel = variationLabel;
    }

    public bool Equals(StateKey other) =>
        StateType == other.StateType && VariationLabel.Equals(other.VariationLabel);

    public override int GetHashCode() =>
        HashCode.Combine(StateType, VariationLabel);
}