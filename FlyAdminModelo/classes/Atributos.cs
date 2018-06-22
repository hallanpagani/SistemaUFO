using System;

namespace BaseModelo.classes
{
    // atributo de campo autoinc
    public class AutoIncAttribute : Attribute
    {
    }

    // atributo que indica campo somente para inserção - não entra no insert
    public class OnlyInsertAttribute : Attribute
    {
    }

    // atributo que indica campo somente para update - não entra na inserção
    public class OnlyUpdateAttribute : Attribute
    {
    }

    // atributo que indica campo somente para selec - não entra na inserção nem edição
    public class OnlySelectAttribute : Attribute
    {
    }
}