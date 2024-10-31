using UnityEngine;
using UnityEngine.UI;

public class CircleText : BaseMeshEffect
{
	public int radius = 100;
	public float spaceCoff = 1f;
	public override void ModifyMesh(VertexHelper vh)
	{
		if (!IsActive() || radius == 0)
		{
			return;
		}

		UIVertex lb = new UIVertex();
		UIVertex lt = new UIVertex();
		UIVertex rt = new UIVertex();
		UIVertex rb = new UIVertex();

		for (int i = 0; i < vh.currentVertCount / 4; i++)
		{
			vh.PopulateUIVertex(ref lb, i * 4);
			vh.PopulateUIVertex(ref lt, i * 4 + 1);
			vh.PopulateUIVertex(ref rt, i * 4 + 2);
			vh.PopulateUIVertex(ref rb, i * 4 + 3);

			Vector3 center = Vector3.Lerp(lb.position, rt.position, 0.5f);
			Matrix4x4 move = Matrix4x4.TRS(center * -1, Quaternion.identity, Vector3.one);
			float rad = Mathf.PI / 2 - center.x * spaceCoff / radius;
			Vector3 pos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
			Quaternion rotation = Quaternion.Euler(0, 0, rad * 180 / Mathf.PI - 90);
			Matrix4x4 rotate = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
			Matrix4x4 place = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
			Matrix4x4 transform = place * rotate * move;

			lb.position = transform.MultiplyPoint(lb.position);
			lt.position = transform.MultiplyPoint(lt.position);
			rt.position = transform.MultiplyPoint(rt.position);
			rb.position = transform.MultiplyPoint(rb.position);
			lb.position.y = lb.position.y - radius + center.y;
			lt.position.y = lt.position.y - radius + center.y;
			rt.position.y = rt.position.y - radius + center.y;
			rb.position.y = rb.position.y - radius + center.y;

			vh.SetUIVertex(lb, i * 4);
			vh.SetUIVertex(lt, i * 4 + 1);
			vh.SetUIVertex(rt, i * 4 + 2);
			vh.SetUIVertex(rb, i * 4 + 3);
		}
	}
}