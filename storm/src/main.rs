use chrono::Local;
use clap::{Parser, Subcommand};
use colored::*;
use std::fs;
use std::path::{Path, PathBuf};

const TRASH_DIR: &str = ".trash_can";

#[derive(Parser)]
#[command(name = "storm")]
#[command(about = "Simple trash system with versioning", long_about = None)]
struct Cli {
    /// Show trash history with IDs and contents
    #[arg(long)]
    history: bool,

    /// Restore a file/directory from a specific ID or version
    #[arg(long, num_args = 2, value_names = ["ID/VERSION", "FILENAME"]
    restore: Option<Vec<String>>,

    /// Files or directories to move to trash
    files: Vec<String>,
}

fn get_versions() -> Vec<String> {
    if !Path::new(TRASH_DIR).exists() {
        return Vec::new();
    }
    let mut versions: Vec<String> = fs::read_dir(TRASH_DIR)
        .unwrap()
        .filter_map(|entry| entry.ok().and_then(|e| e.file_name().into_string().ok()))
        .collect();
    versions.sort();
    versions
}

fn trash(files: &[String]) {
    if !Path::new(TRASH_DIR).exists() {
        fs::create_dir_all(TRASH_DIR).expect("Failed to create trash directory");
    }

    let version = Local::now().format("%Y-%m-%d_%H-%M-%S").to_string();
    let version_dir = Path::new(TRASH_DIR).join(&version);
    fs::create_dir_all(&version_dir).expect("Failed to create version directory");

    for file_path in files {
        let path = Path::new(file_path);
        if path.exists() {
            let dest = version_dir.join(path.file_name().unwrap());
            fs::rename(path, dest).expect("Failed to move file to trash");
            println!(
                "{} moved '{}' to trash (Version: {})",
                "Moved".red(),
                file_path,
                version
            );
        } else {
            println!("Error: File '{}' not found.", file_path);
        }
    }
}

fn history() {
    let versions = get_versions();
    if versions.is_empty() {
        println!("No history available.");
        return;
    }

    println!("{:<4} | {:<20} | {}", "ID", "Version (Timestamp)", "Files");
    println!("{}", "-".repeat(70));

    let mut version_list: Vec<(usize, String)> = versions
        .into_iter()
        .enumerate()
        .map(|(i, v)| (i + 1, v))
        .collect();
    version_list.reverse();

    for (i, (idx, version)) in version_list.iter().enumerate() {
        let v_path = Path::new(TRASH_DIR).join(version);
        let contents: Vec<String> = fs::read_dir(v_path)
            .unwrap()
            .filter_map(|entry| entry.ok().and_then(|e| e.file_name().into_string().ok()))
            .collect();
        let contents_str = contents.join(", ");

        let line = format!(
            "{:<4} | {:<20} | {}",
            idx.to_string().red(),
            version,
            contents_str
        );

        if i == 0 {
            println!("{} {} (LAST DELETED)", line, "(LAST DELETED)".red());
        } else {
            println!("{}", line);
        }
    }
}

fn restore(id_or_version: &str, filename: &str) {
    let versions = get_versions();
    let mut target_version: Option<String> = None;

    // Try ID mapping first
    if let Ok(idx) = id_or_version.parse::<usize>() {
        if idx > 0 && idx <= versions.len() {
            target_version = Some(versions[idx - 1].clone());
        }
    }

    // If not found by ID, treat as timestamp
    if target_version.is_none() {
        if versions.contains(&id_or_version.to_string()) {
            target_version = Some(id_or_version.to_string());
        }
    }

    match target_version {
        Some(version) => {
            let src = Path::new(TRASH_DIR).join(&version).join(filename);
            if src.exists() {
                let dest = Path::new(filename).file_name().unwrap();
                fs::rename(&src, dest).expect("Failed to restore file");
                println!("Restored {} from {}", filename, version);

                // Remove version dir if empty
                let v_path = Path::new(TRASH_DIR).join(&version);
                if fs::read_dir(&v_path).unwrap().next().is_none() {
                    fs::remove_dir(&v_path).expect("Failed to remove empty version directory");
                }
            } else {
                println!("Error: {} in version {} not found.", filename, version);
            }
        }
        None => {
            println!("Error: Version or ID '{}' not found.", id_or_version);
        }
    }
}

fn main() {
    let cli = Cli::parse();

    if cli.history {
        history();
    } else if let Some(restore_args) = cli.restore {
        if restore_args.len() == 2 {
            restore(&restore_args[0], &restore_args[1]);
        }
    } else if !cli.files.is_empty() {
        trash(&cli.files);
    } else {
        // Show help if no arguments
        use clap::CommandFactory;
        Cli::command().print_help().unwrap();
    }
}
