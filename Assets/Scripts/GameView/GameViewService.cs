using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameViewService : BaseSingleService<GameViewService>,IUpdate
{

    class EntityView 
    {
        private IEntity m_entity;
        private PositionComponent m_position;
        private RotateComponent m_rotate;
        private Transform transform;
        public EntityView(IEntity entity,Transform trans) 
        {
            m_entity = entity;
            transform = trans;
            m_position = entity.GetComponent<PositionComponent>(ComponentRegister.GetComponentIndex<PositionComponent>());
            m_rotate = entity.GetComponent<RotateComponent>(ComponentRegister.GetComponentIndex<RotateComponent>());
        }

        public void Update(float deltaTime) 
        {
            if (!m_entity.IsEnable)
                return;
            Vector3 pos = new Vector3(m_position.Position.x.ToFloat(), 0, m_position.Position.y.ToFloat());
            transform.position = Vector3.Lerp(transform.position,pos,0.3f);
            if (m_rotate.Forward != Lockstep.Math.LVector2.zero)
            {
                Quaternion rot = Quaternion.LookRotation(new Vector3(m_rotate.Forward.x.ToFloat(), 0, m_rotate.Forward.y.ToFloat())
                    , Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.3f);
            }
        }
    }

    private List<EntityView> m_views = new List<EntityView>();

    public void CreatView(IEntity entity,string path,bool isMainPlayer = false) 
    {
        ResourceService.Instance.LoadGameObjectAsync(path, (go) => 
        {
            if (!entity.IsEnable)
            {
                ResourceService.Instance.ReleaseAsset(go);
                return;
            }

            m_views.Add(new EntityView(entity,go.transform));
            if (isMainPlayer) 
            {
                CameraService.Instance.SetTarget(go.transform);
            }
        });
    }

    public void DoUpdate(float deltaTime)
    {
        for (int i = 0; i < m_views.Count; i++)
        {
            m_views[i].Update(deltaTime);
        }
    }
}
