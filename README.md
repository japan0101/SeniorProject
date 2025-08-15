# Senior-project-game
 
## Setting up environment For MLAgent
You can follow These instructions from the Unity\
https://unity-technologies.github.io/ml-agents/Installation/

or

mine

### Make sure you have Microsoft Visual C++ Redistributable
https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170

### Install CUDA Toolkit (Not essential for GPU without CUDA cores)
Check CUDA GPU Compute Capability\
https://developer.nvidia.com/cuda-gpus

Download the CUDA Toolkit 12.1.1 and proceed with the installation\
https://developer.nvidia.com/cuda-12-1-1-download-archive

Check the cuDNN Compatibility\
https://docs.nvidia.com/deeplearning/cudnn/backend/latest/reference/support-matrix.html

Download the cuDNN for CUDA Toolkit 12.1.1 and proceed with the installation\
https://developer.nvidia.com/cudnn-downloads?target_os=Windows&target_arch=x86_64&target_version=11

Download Anaconda

"what is anaconda?"\
It's a distribution of Python and R, primarily focused on data science, machine learning, and scientific computing.\
to simplify its just Python that's made for data science ecosystem

we didn't need the entire Anaconda\
so we'll use Miniconda\
https://www.anaconda.com/docs/getting-started/miniconda/main

"I thought we use Unity MLAgents Why are we installing Python?"\
Well the training component of MLAgents is implemented in Python\
 even though Unity uses C#\
 Also python is one of the most popular languages for machine learning\
 offering libraries such as TensorFlow and PyTorch Both of these are\
 absolute powerhouse that simplify AI development

MLAgents uses pytorch running in Python to handle an essential task like
training, data processing and updating the model through reinforcement learning

Reinforcement learning is a type of machine learning where agents learn by\
interacting with their environment and receiving rewards for achieving specific\
goals pytorch manages the complex calculation involved in this learning process\
while Unity provides the environment where agents interact, learn and adapt in real time.

This setup lets Python handle the training side while Unity manages the visual and interactive\
parts of the simulation

Let's proceed with the installation\
boot up the anaconda PowerShell that we're installed earlier

Create a new Conda environment with Python 3.10.12
```powershell
conda create -n mlagents python=3.10.12
```

Activate the environment
```powershell
conda activate mlagents
```

Install the necessary packages
```powershell
conda install numpy=1.23.5
pip3 install torch~=2.2.1 --index-url https://download.pytorch.org/whl/cu121
```
if you didn't have GPU with CUDA cores
```powershell
pip3 install torch~=2.2.1 --index-url https://download.pytorch.org/whl/cpu
```

Start Python to verify installation
```python
python
import torch
import numpy
print(torch.__version__)
print(numpy.__version__)
exit()
```
Change directory to this project
```powershell
cd [path_to_this_project]
```

Install ML-Agents from the local source files
```powershell
python -m pip install ./ml-agents-envs
python -m pip install ./ml-agents
```
Check ML-Agents installation
```powershell
mlagents-learn --help
```
Start a training session

```powershell
mlagents-learn ./config/[cofigfilename.yml] --run-id=[run_id]
```
Note: if you didn't specify the config, it'll use the default config and run_id must always be specified

Force overwrite or resume previous run
```powershell
mlagents-learn ./config/[cofigfilename.yml] --run-id=[same_run_id] --force
mlagents-learn ./config/[cofigfilename.yml] --run-id=[same_run_id] --resume
```