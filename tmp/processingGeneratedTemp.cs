TextFragment processing1(TextFragment all)
{
    int level = 0;
    int current = 0;
    bool emptyline = true;
    char tabulationtype = 'x';
    for (int p1 = 0; p1 < all.Lines.Count; p1++)
    {
        var line = all.Lines[p1];

        emptyline = true;
        current = 0;
        tabulationtype = 'x';

        for (int p2 = 0; p2 < line.Characters.Count; p2++)
        {
            var character = line.character[p2];
            if (character.Is("#"))
            {
                character.Break(character);
                break;
            }
            if (!character.Is("\t") && !character.Is("\n") && !character.Is(" "))
            {
                emptyline = false;
            }
        }

        if (emptyline)
        {
            line.Jump();
            continue;
        }

        for (int p2 = 0; p2 < line.Characters.Count; p2++)
        {
            var character = line.character[p2];
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

        for (int p1 = 0; p1 < line.Lines.Count; p1++)
        {
            var line2 = all.Lines[p1];
            if (current < level)
            {
                level = current;
                line2.Return("@startblock" + "\n" + line2);
            }
        }

        while (level > current)
        {
            level -= 2;
            for (int p1 = 0; p1 < line.Lines.Count; p1++)
            {
                var line2 = all.Lines[p1];
                line2.Return("@endblock" + "\n" + line2);
            }
        }

        if (emptyline)
        {
            continue;
        }
        line.Return(line + " @endline");
    }
    all.Return(all + " @endfile");
    return all;
}