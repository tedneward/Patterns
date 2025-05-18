public interface KnowledgeSource {
    public boolean execCondition(Blackboard bb);
    public Object execAction(Blackboard bb);
}
