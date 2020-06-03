/**
 * ===============
 * format_util.hpp
 * ===============
 *
 * Anguillifax NovaLib
 * Utility Functions
 *
 * A variety of string formatting shorthands.
 *
 * These include
 * - General purpose string conversion functions
 * - Converting containers into comma separated strings
 * - Converting maps into key-value pairs
 * - Formatting functions that support writing to any given output stream
 *
 * Most functions come in pairs: one function that appends a newline and one that doesn't.
 * Functions that append newlines have "ln" following their name.
 *
 * Useful for non performance critical sections of code.
 */

#ifndef NOVA_FORMAT_UTIL_HPP
#define NOVA_FORMAT_UTIL_HPP


#include <algorithm>
#include <iostream>
#include <string>
#include <sstream>
#include <vector>


namespace nova {

	// =========
	// Stringify
	// =========

	/**
	 * Convert an object into a string using the << stream operator.
	 */
	template<typename T>
	inline std::string stringify(const T& item)
	{
		std::stringstream o;
		o << item;
		return o.str();
	}

	/**
	 * Convert a boolean into a string.
	 */
	template<>
	inline std::string stringify(const bool& item)
	{
		return item ? "true" : "false";
	}


	// =============
	// Comma Strings
	// =============

	/**
	 * Convert the contents of a container into a list of items separated by commas.
	 *
	 * TContainer must support begin() and end().
	 */
	template<typename TContainer>
	inline std::string to_comma_string(const TContainer& container, const std::string& separator = ", ")
	{
		std::ostringstream o;
		auto it = container.begin();
		auto end = container.end();
		while (true) {
			o << *it;
			++it;
			if (it != end)
				o << separator;
			else
				return o.str();
		}
	}


	// ================
	// Headered Strings
	// ================

	/**
	 * Convert a container into a string with a header and indented contents.
	 *
	 * TContainer must support begin() and end().
	 */
	template<typename TContainer>
	inline std::string to_header_string(const std::string& header, const TContainer& container, const std::string& indent = "  ")
	{
		std::ostringstream o;
		o << header;

		for (const auto& item : container) {
			o << std::endl;
			o << indent << item;
		}

		return o.str();
	}

	/**
	 * Convert a container into a string with a header and indented key-value pairs.
	 *
	 * TMap must support begin() and end().
	 *
	 * The object returned by begin() must support it->first and it->second.
	 */
	template<typename TMap>
	inline std::string to_unsorted_map_string(const std::string& header, const TMap& map, const std::string& indent = "  ")
	{
		std::ostringstream o;
		o << header;

		for (const auto& it : map) {
			o << std::endl;
			o << indent << it->first << ": " << it->second;
		}

		return o.str();
	}

	/**
	 * Convert a container into a string with a header and sorted, indented key-value pairs.
	 *
	 * TMap must support begin() and end().
	 *
	 * The object returned by begin() must support it->first and it->second.
	 *
	 * it->first must be sortable.
	 */
	template<typename TMap>
	inline std::string to_map_string(const std::string& header, const TMap& map, const std::string& indent = "  ")
	{
		using TIterator = decltype(map.begin());

		std::vector<TIterator> v;
		for (auto iterator = map.begin(); iterator != map.end(); ++iterator)
			v.push_back(iterator);
		std::sort(v.begin(), v.end(), [](const TIterator& x, const TIterator& y) { return x->first < y->first; });

		std::ostringstream o;
		o << header;

		for (const auto& it : v) {
			o << std::endl;
			o << indent << it->first << ": " << it->second;
		}

		return o.str();
	}


	// ==============================
	// Formatted Strings and Printing
	// ==============================

	namespace {

		/**
		 * Recursively iterates through a string and replace occurrences of {} with a value.
		 *
		 * Note: s is mutated during execution.
		 */
		template<typename TFirst, typename... TTail>
		void _format_recursive(std::ostream& output, std::string& s, const TFirst& first, const TTail& ... tail)
		{
			size_t next_split_index = s.find("{}");

			if (next_split_index != std::string::npos) {

				output << std::string_view(s.data(), next_split_index) << first;
				s.erase(s.begin(), s.begin() + next_split_index + 2);

				if constexpr(sizeof...(tail) > 0)
					_format_recursive(output, s, tail...);
			}

			output << s;
		}

	}

	/**
	 * Return a new string from the given format.
	 *
	 * Use {} to indicate where a value should be inserted.
	 */
	template<typename TFirst, typename... TTail>
	std::string format_to_string(std::string format, const TFirst& first, const TTail& ... tail)
	{
		std::ostringstream o;
		_format_recursive(o, format, first, tail...);
		return o.str();
	}

	/**
	 * Return a new string from the given format with a trailing newline.
	 *
	 * Use {} to indicate where a value should be inserted.
	 */
	template<typename TFirst, typename... TTail>
	std::string formatln_to_string(std::string format, const TFirst& first, const TTail& ... tail)
	{
		std::ostringstream o;
		_format_recursive(o, format, first, tail...);
		o << std::endl;
		return o.str();
	}

	/**
	 * Write a formatted string to the output destination.
	 *
	 * Use {} to indicate where a value should be inserted.
	 */
	template<typename TFirst, typename... TTail>
	void format_to(std::ostream& output, std::string format, const TFirst& first, const TTail& ... tail)
	{
		_format_recursive(output, format, first, tail...);
	}

	/**
	 * Write a formatted string to the output destination with a trailing newline.
	 *
	 * Use {} to indicate where a value should be inserted.
	 */
	template<typename TFirst, typename... TTail>
	void formatln_to(std::ostream& output, std::string format, const TFirst& first, const TTail& ... tail)
	{
		_format_recursive(output, format, first, tail...);
		output << std::endl;
	}


	/**
	 * Print a formatted string to standard out.
	 *
	 * Use {} to indicate where a value should be inserted.
	 */
	template<typename TFirst, typename... TTail>
	inline void print(const std::string& format, const TFirst& first, const TTail& ... tail)
	{
		format_to(std::cout, format, first, tail...);
	}

	/**
	 * Print a formatted string to standard out with a trailing newline.
	 *
	 * Use {} to indicate where a value should be inserted.
	 */
	template<typename TFirst, typename... TTail>
	inline void println(const std::string& format, const TFirst& first, const TTail& ... tail)
	{
		print(format, first, tail...);
		std::cout << std::endl;
	}

	/**
	 * Print a string to standard out.
	 */
	inline void print(const std::string& format)
	{
		std::cout << format;
	}

	/**
	 * Print a string to standard out with a trailing newline.
	 */
	inline void println(const std::string& format)
	{
		std::cout << format << std::endl;
	}


} // namespace nova


#endif //NOVA_FORMAT_UTIL_HPP
