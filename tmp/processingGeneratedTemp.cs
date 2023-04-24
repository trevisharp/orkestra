Key ENDFILE = new Key();
Key ENDLINE = new Key();
Key STARTBLOCK = new Key();
Key ENDBLOCK = new Key();

// ...

Text processing1(Text txt)
{
    int level = 0;
    int current = 0;
    bool emptyline = true;
    char tabulationtype = 'x';

    while (txt.NextLine())
    {
        emptyline = true;
        current = 0;
        tabulationtype = 'x';

        while (txt.NextCharacterLine())
        {
            var character = txt.Current;
            if (txt.Is("#"))
            {
                character.Break();
                break;
            }

            if (!character.Is("\t") && !character.Is("\n") && !character.Is(" "))
            {
                emptyline = false;
            }
        }
        txt.PopProcessing();

        if (emptyline)
        {
            line.Discard();
            continue;
        }

        while (txt.NextCharacterLine())
        {
            if (tabulationtype == 'x')
            {
                if (character.Is("\t") || character.Is(" "))
                {
                    tabulationtype = character;
                }
                else
                {
                    continue;
                }
                if (!character.Is(tabulationtype))
                {
                    character.Complete();
                    break;
                }
                if (character.Is("\t"))
                {
                    current += 2;
                }
                else if (character.Is(" "))
                {
                    current += 1;
                }
            }
        }
        txt.PopProcessing();

        if (current < level)
        {
            level = current;
            line2.Prepend(STARTBLOCK, "\n");
        }

        while (level > current)
        {
            level -= 2;
            line.Append(ENDBLOCK, "\n");
        }

        if (emptyline)
            continue;
        
        line.Append(ENDLINE);
    }
    txt.PopProcessing();
    all.Append(ENDFILE);
    return all;
}