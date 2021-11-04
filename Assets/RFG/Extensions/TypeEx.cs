using System;

public static class TypeEx
{
  public static Type GetType(string TypeName)
  {

    // Try Type.GetType() first. This will work with types defined
    // by the Mono runtime, in the same assembly as the caller, etc.
    var type = Type.GetType(TypeName);

    // If it worked, then we're done here
    if (type != null)
      return type;

    // Attempt to load the indicated Assembly
    System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
    foreach (var assembly in AS)
    {
      // Ask that assembly to return the proper Type
      type = assembly.GetType(TypeName);
      if (type != null)
        return type;
    }

    // The type just couldn't be found...
    return null;

  }
}
