# VCS Performance Comparison using SHAP and Machine Learning

This repository provides a machine learning-based performance analysis of two popular distributed version control systems: **Git** and **Mercurial**. The study investigates how key repository features impact CPU time, memory usage, and storage space using **Random Forest** and **SHAP** (SHapley Additive exPlanations).

## Purpose
The goal is to help developers and organizations make informed decisions when selecting a version control system, based on measurable performance characteristics and interpretable machine learning insights.

## Key Features
- Synthetic repository generation to simulate diverse project structures.
- Resource usage benchmarking for Git and Mercurial across CPU, memory, and storage.
- CPU Time, Memory Usage and Storage Space Measures 

## Technologies Used
- C#.NET
- Git & Mercurial command-line tools

## Repository Structure

## Highlights from the Study
- Git showed more efficient CPU and memory usage, especially during branch operations.
- Mercurial exhibited better storage consistency and resilience to file structure.
- SHAP analysis revealed strong impacts of repository size and commit frequency on performance.
- Clear trade-offs between Git’s processing consistency and Mercurial’s storage optimization.

- 
