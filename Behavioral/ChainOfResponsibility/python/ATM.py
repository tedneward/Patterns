class ATMHandler:
    def __init__(self):
        self._next_handler = None

    def set_next_handler(self, handler):
        self._next_handler = handler

    def handle_request(self, amount):
        raise NotImplementedError("This method should be overridden by subclasses")

class FiftyDollarHandler(ATMHandler):
    def handle_request(self, amount):
        if amount >= 50:
            count = amount // 50
            amount -= count * 50
            print(f"Dispensing {count} x $50 bills")

        if amount > 0 and self._next_handler:
            self._next_handler.handle_request(amount)

class TwentyDollarHandler(ATMHandler):
    def handle_request(self, amount):
        if amount >= 20:
            count = amount // 20
            amount -= count * 20
            print(f"Dispensing {count} x $20 bills")

        if amount > 0 and self._next_handler:
            self._next_handler.handle_request(amount)

class TenDollarHandler(ATMHandler):
    def handle_request(self, amount):
        if amount >= 10:
            count = amount // 10
            amount -= count * 10
            print(f"Dispensing {count} x $10 bills")

        if amount > 0 and self._next_handler:
            self._next_handler.handle_request(amount)

class ATM:
    def __init__(self):
        fifty = FiftyDollarHandler()
        twenty = TwentyDollarHandler()
        ten = TenDollarHandler()

        self._chain = fifty
        fifty.set_next_handler(twenty)
        twenty.set_next_handler(ten)

    def dispense(self, amount):
        self._chain.handle_request(amount)

atm = ATM()
while True:
    try:
        amount = int(input("Enter the amount to withdraw (or 0 to exit): "))
        if amount == 0:
            break
        if amount % 10 != 0:
            raise ValueError("Amount must be a multiple of 10.")
        atm.dispense(amount)
    except ValueError:
        print("Invalid input. Please enter a valid amount.")

