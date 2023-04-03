#!/usr/bin/env python3

import sys
import time

counter = 0

start = time.time()

try:
    while True:
        print(f"\rerror: {counter}", file=sys.stderr, flush=True, end='')
        counter += 1
        #time.sleep(0.001)
except KeyboardInterrupt:
    elapsed_ms = (time.time() - start) * 1000
    print()
    print(f"elapsed: {elapsed_ms} {elapsed_ms / counter}")
