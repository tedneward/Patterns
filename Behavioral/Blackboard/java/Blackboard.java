import java.util.ArrayList;
import java.util.List;

public class Blackboard {

    public final List<KnowledgeSource> sources = new ArrayList<>();

    private Object state;

    public Object inspect() { return state; }
    public void update(Object newState) { state = newState; }

    public void process(Object state) {

    }

}