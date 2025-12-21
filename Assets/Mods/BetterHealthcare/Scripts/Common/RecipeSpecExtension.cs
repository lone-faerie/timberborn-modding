using Timberborn.Workshops;

namespace Mods.BetterHealthcare.Scripts.Common
{
    public static class RecipeSpecExtension
    {
        public static bool IsEffect(this RecipeSpec recipeSpec)
        {
            return recipeSpec.Id.StartsWith("Effect.");
        }
    }
}