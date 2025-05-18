public class Driver {
    public static void main(String... args) {
        Blackboard board = new Blackboard();

        KnowledgeSource ks1 = new KnowledgeSource() {

            @Override
            public void run() {
                // TODO Auto-generated method stub
                
            }

            @Override
            public boolean canHandle(BlackboardObject bbo, Blackboard bb) {
                // TODO Auto-generated method stub
                return false;
            }

            @Override
            public BlackboardObject process(BlackboardObject bbo) throws Exception {
                // TODO Auto-generated method stub
                return null;
            }

            @Override
            public void update(BlackboardObject bbo) {
                // TODO Auto-generated method stub
                
            }
            
        };

        board.addObserver(ks1);
    }
}
