//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGenerator.MatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Entitas;

public sealed partial class GameMatcher {

    static IMatcher<GameEntity> _matcherHideInBlueprintInspector;

    public static IMatcher<GameEntity> HideInBlueprintInspector {
        get {
            if(_matcherHideInBlueprintInspector == null) {
                var matcher = (Matcher<GameEntity>)Matcher<GameEntity>.AllOf(GameComponentsLookup.HideInBlueprintInspector);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherHideInBlueprintInspector = matcher;
            }

            return _matcherHideInBlueprintInspector;
        }
    }
}