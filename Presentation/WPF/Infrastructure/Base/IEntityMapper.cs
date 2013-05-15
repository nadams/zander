namespace Zander.Presentation.WPF.Zander.Infrastructure.Base {
	public interface IEntityMapper<Entity, Model> {
		Model ModelFromEntity(Entity e);
		Entity EntityFromModel(Model m);
	}
}
