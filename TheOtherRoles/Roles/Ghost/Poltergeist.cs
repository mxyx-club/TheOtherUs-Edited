namespace TheOtherRoles.Roles.Ghost;
public class Poltergeist
{
    public static Color color = new Color32(210, 220, 234, byte.MaxValue);

    public static PlayerControl Player;
    public static DeadBody targetBody;

    public static int cooldown;
    public static float radius;

    public static Sprite ButtonSprite = new ResourceSprite("PoltergeistButton.png");

#nullable enable
    public static void MoveDeadBody(byte targetId, Vector2 pos) => Coroutines.Start(CoMoveDeadBody(GetDeadBody(targetId), pos));

    public static IEnumerator CoMoveDeadBody(DeadBody? deadBody, Vector2 pos)
    {
        if (deadBody == null)
        {
            yield break;
        }

        float p = 0f;
        Vector2 beginPos = deadBody.transform.position;

        while (deadBody)
        {
            p += Time.deltaTime * 0.85f;
            if (!(p < 1f)) break;

            float pp = p * p;
            Vector3 currentPos = beginPos * (1 - pp) + pos * pp;
            currentPos.z = currentPos.y / 1000f;
            deadBody.transform.position = currentPos;

            yield return null;
        }

        if (deadBody) deadBody.transform.position = AsVector3(pos, pos.y / 1000f);

        yield break;
        static Vector3 AsVector3(Vector2 vec, float z)
        {
            Vector3 result = vec;
            result.z = z;
            return result;
        }
    }
#nullable disable

    public static void ClearAndReload()
    {
        Player = null;
        targetBody = null;
        cooldown = CustomOptionHolder.poltergeistCooldown.GetInt();
        radius = CustomOptionHolder.poltergeistRadius.GetFloat();
    }
}
