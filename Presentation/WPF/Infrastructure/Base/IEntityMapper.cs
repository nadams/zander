namespace Zander.Presentation.WPF.Zander.Infrastructure.Base {
	public interface IEntityMapper<Entity, Model> {
		Model ModelFromEntity(Entity e);
		Entity EntityFromModel(Model m);
        void CopyModel(Model m1, Model m2);
	}
}
