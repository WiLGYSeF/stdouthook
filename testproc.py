#!/usr/bin/env python3

import sys
import time

counter = 0

start = time.time()

try:
    while True:
        print(f"output {counter}", file=sys.stdout, flush=True)
        print(f"error  {counter}", file=sys.stderr, flush=True)
        counter += 1
        #time.sleep(0.001)
except KeyboardInterrupt:
    elapsed_ms = (time.time() - start) * 1000
    print(f"elapsed: {elapsed_ms} {elapsed_ms / counter}")
