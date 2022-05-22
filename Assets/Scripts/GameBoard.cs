using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EzHexMapHexagonal))]
public class GameBoard : MonoBehaviour
{
    public EzHexResident tilePrefab;
    public GamePiece gamePiecePrefab;
    public SlideButton slideButtonPrefab;

    public EzHexMapHexagonal map;

    public SpriteRenderer fade;

    private List<EzHexResident> tiles;
    private List<GamePiece> pieces;

    public IEzSeq currentSequence = null;

    public Vector3Int pendingMove = Vector3Int.zero;

    public float speed = 0.15f;

    public Color tileNormal = Color.gray, tileFade = Color.gray;

    public Vector3Int currentHighlight = Vector3Int.zero;
    public Vector3Int hoverHighlight = Vector3Int.zero;

    private string[] keys = { "D", "E", "W", "A", "Z", "X" };

    private void Awake()
    {
        map = GetComponent<EzHexMapHexagonal>();
    }

    // Start is called before the first frame update
    void Start()
    {
        tiles = map.Fill(tilePrefab);
        pieces = new List<GamePiece>();

        for (int i = 0; i < 6; ++i)
        {
            Vector3Int dir = EzHexMap.directions[i];

            SlideButton btn = Instantiate(slideButtonPrefab, map.map.CellToLocal(dir * 3), Quaternion.identity, transform);
            btn.onClick = OnButtonPressed;
            btn.onHover = OnButtonHover;
            btn.onLeave = OnButtonLeave;
            btn.direction = dir;
            btn.text.text = keys[i];
            btn.icon.localRotation = EzHexMap.orientations[i];
        }

        Reset();

        OnButtonLeave(Vector3Int.zero);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            OnButtonPressed(EzHexMap.upLeft);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            OnButtonPressed(EzHexMap.upRight);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            OnButtonPressed(EzHexMap.left);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            OnButtonPressed(EzHexMap.right);
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            OnButtonPressed(EzHexMap.downLeft);
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            OnButtonPressed(EzHexMap.downRight);
        }


        if (currentSequence != null)
        {
            if (currentSequence.Done())
            {
                currentSequence = null;
            }
            else
            {
                currentSequence.Update(Time.deltaTime);
            }
        }
        else
        {
            if (hoverHighlight != currentHighlight)
            {
                if (hoverHighlight == Vector3Int.zero)
                {
                    ClearHighlight();
                }
                else
                {
                    Highlight(hoverHighlight);
                }
            }

            if (pendingMove != Vector3Int.zero)
            {
                ExecuteMove(pendingMove);
                pendingMove = Vector3Int.zero;
            }
        }
    }

    private void Highlight(Vector3Int direction)
    {
        List<EzHexLine> lines = GetLines(direction);


        foreach (EzHexResident tile in tiles)
        {
            SpriteRenderer r = tile.GetComponent<SpriteRenderer>();
            r.color = tileFade;

            foreach (EzHexLine line in lines)
            {
                if (line.Contains(tile.position))
                {
                    r.color = tileNormal;
                    break;
                }
            }
        }

        currentHighlight = direction;
    }

    void OnButtonHover(Vector3Int direction)
    {
        hoverHighlight = direction;
    }

    private void ClearHighlight()
    {
        foreach (EzHexResident tile in tiles)
        {
            SpriteRenderer r = tile.GetComponent<SpriteRenderer>();
            r.color = tileNormal;
        }

        currentHighlight = Vector3Int.zero;
    }

    void OnButtonLeave(Vector3Int direction)
    {
        hoverHighlight = Vector3Int.zero;
    }

    private List<IEzSeq> GetMoveSequence(Vector3Int direction)
    {
        List<IEzSeq> seqs = new List<IEzSeq>();
        List<EzHexLine> lines = GetLines(direction);

        for (int i = 0; i < lines.Count; ++i)
        {
            IEzSeq lineSeq = HandleLineSlide(lines[i]);
            if (lineSeq != null)
            {
                seqs.Add(lineSeq);
            }
        }

        return seqs;
    }

    private void ExecuteMove(Vector3Int direction)
    {
        List<EzHexLine> lines = GetLines(direction);
        List<IEzSeq> seqs = GetMoveSequence(direction);

        if (seqs.Count > 0)
        {
            currentSequence =
                EzSequence.Create(
                    EzSeqParallel.Create(seqs.ToArray()),
                    new SpawnSeq() { lines = lines, board = this },
                    new CheckDefeatSeq() { board = this });
            currentSequence.Begin();

            Highlight(direction);
        }
    }

    void OnButtonPressed(Vector3Int direction)
    {
        if (currentSequence != null)
        {
            pendingMove = direction;
            return;
        }


        ExecuteMove(direction);

    }

    public void Spawn(List<EzHexLine> lines)
    {
        List<Vector3Int> candidates = new List<Vector3Int>();

        foreach (EzHexLine line in lines)
        {
            if (GetPiece(line.start) == null)
            {
                candidates.Add(line.start);
            }
        }

        if (candidates.Count > 0)
        {
            AddPiece(candidates[Random.Range(0, candidates.Count)], Random.Range(0, 8) == 0 ? 1 : 0);
        }
    }

    private class MoveHexGrid : IEzSeq
    {
        public GamePiece piece;
        public Vector3Int step;

        public void Begin()
        {
            piece.resident.position += step;
            piece = null;
        }

        public bool Done()
        {
            return piece == null;
        }

        public void Update(float deltaT)
        {
        }
    }

    private class SpawnSeq : IEzSeq
    {
        public GameBoard board;
        public List<EzHexLine> lines;
        public void Begin()
        {
            board.Spawn(lines);
            board = null;
        }

        public bool Done()
        {
            return board == null;
        }

        public void Update(float deltaT)
        {
        }
    }

    private class MergeComplete : IEzSeq
    {
        public GamePiece source, target;
        public GameBoard board;
        public void Begin()
        {
            target.level++;

            board.RemovePiece(source);

            GameObject.Destroy(source.gameObject);
            board = null;
        }

        public bool Done()
        {
            return board == null;
        }

        public void Update(float deltaT)
        {
        }
    }

    private class CheckDefeatSeq : IEzSeq
    {
        public GameBoard board;

        public void Begin()
        {
            board.CheckDefeat();
            board = null;
        }

        public bool Done()
        {
            return board == null;
        }

        public void Update(float deltaT)
        {
        }
    }

    IEzSeq CreateMoveSeq(GamePiece piece, Vector3Int move, float time)
    {
        return EzSeqParallel.Create(
                new MoveHexGrid() { piece = piece, step = move},
                EzSeqLerpPosition.CreateRelative(piece.transform, map.map.CellToLocal(move), time));
    }

    IEzSeq CreateMergeSeq(float delay, GamePiece source, GamePiece target)
    {
        SpriteRenderer[] renderers = source.GetComponentsInChildren<SpriteRenderer>();

        List<IEzSeq> fades = new List<IEzSeq>();
        foreach (SpriteRenderer r in renderers)
        {
            fades.Add(EzSeqLerpColor.Create(r, Color.clear, speed));
        }

        return EzSequence.Create(
            EzSeqDelay.Create(delay),
            EzSeqParallel.Create(fades.ToArray()),
            new MergeComplete() { source = source, target = target, board = this }
        );
    }

    IEzSeq HandleLineSlide(EzHexLine line)
    {
        int free = 0;

        List<IEzSeq> seqs = new List<IEzSeq>();

        GamePiece mergeCandidate = null;

        for (int i = 0; i < line.length; ++i)
        {
            Vector3Int position = line.GetTile(line.length - (i + 1));

            GamePiece piece = GetPiece(position);
            if (!piece)
            {
                ++free;
                continue;
            }

            if (mergeCandidate && mergeCandidate.level == piece.level)
            {
                seqs.Add(CreateMergeSeq(speed * free, piece, mergeCandidate));

                ++free;

                Vector3Int move = line.direction * free;
                seqs.Add(EzSeqLerpPosition.CreateRelative(piece.transform, map.map.CellToLocal(move), speed * free));

                mergeCandidate = null;

                continue;
            }

            if (free > 0)
            {
                Vector3Int move = line.direction * free;

                seqs.Add(CreateMoveSeq(piece, move, speed * free));
            }

            mergeCandidate = piece;
        }

        return seqs.Count > 0 ? EzSeqParallel.Create(seqs.ToArray()) : null;
    }

    GamePiece GetPiece(Vector3Int position)
    {
        foreach (GamePiece piece in pieces)
        {
            if (piece.resident.position == position)
            {
                return piece;
            }
        }

        return null;
    }

    void AddPiece(Vector3Int position, int level = 0)
    {
        GamePiece obj = Instantiate(gamePiecePrefab, map.map.CellToLocal(position), Quaternion.identity, transform);
        obj.level = level;
        obj.resident.position = position;

        pieces.Add(obj);
    }

    void RemovePiece(GamePiece piece)
    {
        pieces.Remove(piece);
    }

    private void Reset()
    {
        currentSequence = null;
        foreach (GamePiece piece in pieces)
        {
            Destroy(piece.gameObject);
        }

        pieces.Clear();

        int bonus = Random.Range(0, 3) * 2;

        for (int i = 0; i < 6; ++i)
        {
            Vector3Int dir = EzHexMap.directions[i];

            if (i % 2 == 0)
            {
                AddPiece(dir, i == bonus ? 1 : 0);
            }
        }

        fade.color = new Color(1, 1, 1, 0);
    }

    void CheckDefeat()
    {
        bool defeated = true;

        for (int i = 0; i < 6; ++i)
        {
            if (GetMoveSequence(EzHexMap.directions[i]).Count > 0)
            {
                defeated = false;
                break;
            }
        }

        if (defeated)
        {
            currentSequence = EzSeqLerpColor.Create(fade, new Color(1, 1, 1, 0.75f), 0.5f);
            currentSequence.Begin();
        }
    }

    List<EzHexLine> GetLines(Vector3Int direction)
    {
        List<EzHexLine> result = new List<EzHexLine>();

        int l = map.radius * 2 - 1;

        Vector3Int centerPoint = -direction * (map.radius - 1);
        result.Add(new EzHexLine() { direction = direction, start = centerPoint, length = l });
        result.Add(new EzHexLine() { direction = direction, start = centerPoint + EzHexMap.RotateClockwise(direction), length = l - 1 });
        result.Add(new EzHexLine() { direction = direction, start = centerPoint + EzHexMap.RotateCounterClockwise(direction), length = l - 1 });

        return result;
    }
}
