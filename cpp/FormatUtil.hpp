/**
 * ===============
 * format_util.hpp
 * ===============
 *
 * Anguillifax NovaLib
 * Category: Utility
 */

#ifndef NOVA_FORMAT_UTIL_HPP
#define NOVA_FORMAT_UTIL_HPP

#include <algorithm> // for sorting
#include <iostream>
#include <string>
#include <sstream>
#include <vector> // for sorting


namespace nova {

	/**
	 * Provides a variety of string formatting utilities.
	 *
	 * - Convert a container into a delimiter-separated string.
	 *
	 * - Convert a map into a key-value pairs.
	 *
	 * - Standardized {} style format function. The function can either return either
	 * a new string or write the output to a stream. Shortcuts for standard out are provided.
	 *
	 * Most functions come in pairs: one function that appends a newline and one that doesn't.
	 * Functions that append newlines have "ln" following their name.
	 *
	 * Note: FormatUtil will not set the boolalpha flag on external streams. These must be set
	 * manually before calling the function. In contrast, the string creation functions
	 * always convert booleans using the boolalpha flag.
	 *
	 * TContainer is any type that can be used in a foreach loop.
	 *
	 * TMap is any type that returns an key-value pair where it->first is the key
	 * and it->second is the value.
	 */
	class FormatUtil {
	public:


		// =============
		// Comma Strings
		// =============


		/**
		 * Convert the contents of a container into a list of items separated by a delimiter.
		 * By default, the function will return a comma-separated string.
		 *
		 * TContainer must support begin() and end().
		 */
		template<typename TContainer>
		static std::string to_joined_string(const TContainer& container, const std::string& delimiter = ", ");


		// ================
		// Headered Strings
		// ================


		/**
		 * Convert a container into a string with a header and indented contents.
		 *
		 * TContainer must support begin() and end().
		 */
		template<typename TContainer>
		static std::string to_header_string(const std::string& header, const TContainer& container, const std::string& indent = "  ");

		/**
		 * Convert a container into a string with a header and indented key-value pairs.
		 *
		 * TMap must support begin() and end().
		 *
		 * The object returned by begin() must support it->first and it->second.
		 */
		template<typename TMap>
		static std::string to_unsorted_map_string(const std::string& header, const TMap& map, const std::string& indent = "  ");

		/**
		 * Convert a container into a string with a header and sorted indented key-value pairs.
		 *
		 * TMap must support begin() and end().
		 *
		 * The object returned by begin() must support it->first and it->second.
		 *
		 * it->first must be sortable.
		 *
		 * The iterator must support copy construction.
		 */
		template<typename TMap>
		static std::string to_map_string(const std::string& header, const TMap& map, const std::string& indent = "  ");


		// =================
		// Formatted Strings
		// =================


		/**
		 * Return a new string from the given format.
		 *
		 * Use {} to indicate where a value should be inserted.
		 */
		template<typename TFirst, typename... TTail>
		static std::string format_to_string(std::string format, const TFirst& first, const TTail& ... tail);

		/**
		 * Return a new string from the given format with a trailing newline.
		 *
		 * Use {} to indicate where a value should be inserted.
		 */
		template<typename TFirst, typename... TTail>
		static std::string formatln_to_string(std::string format, const TFirst& first, const TTail& ... tail);

		/**
		 * Write a formatted string to the output destination.
		 *
		 * Use {} to indicate where a value should be inserted.
		 */
		template<typename TFirst, typename... TTail>
		static void format_to(std::ostream& output, std::string format, const TFirst& first, const TTail& ... tail);

		/**
		 * Write a formatted string to the output destination with a trailing newline.
		 *
		 * Use {} to indicate where a value should be inserted.
		 */
		template<typename TFirst, typename... TTail>
		static void formatln_to(std::ostream& output, std::string format, const TFirst& first, const TTail& ... tail);


		// ========
		// Printing
		// ========


		/**
		 * Print a formatted string to standard out.
		 *
		 * Use {} to indicate where a value should be inserted.
		 */
		template<typename TFirst, typename... TTail>
		static void print(const std::string& format, const TFirst& first, const TTail& ... tail);

		/**
		 * Print a formatted string to standard out with a trailing newline.
		 *
		 * Use {} to indicate where a value should be inserted.
		 */
		template<typename TFirst, typename... TTail>
		static void println(const std::string& format, const TFirst& first, const TTail& ... tail);

		/**
		 * Print a string to standard out.
		 *
		 * This function is logically equivalent to print("{}", str).
		 */
		static void print(const std::string& str);

		/**
		 * Print a string to standard out with a trailing newline.
		 *
		 * This function is logically equivalent to println("{}", str).
		 */
		static void println(const std::string& str);

	private:

		/**
		 * Recursively iterates through a string and replace occurrences of {} with a value.
		 *
		 * Note: s is mutated during execution.
		 */
		template<typename TFirst, typename... TTail>
		static void _format_recursive(std::ostream& output, std::string& s, const TFirst& first, const TTail& ... tail);

	}; // class FormatUtil


	// ============
	// Comma String
	// ============


	template<typename TContainer>
	inline std::string FormatUtil::to_joined_string(const TContainer& container, const std::string& delimiter)
	{
		std::ostringstream o;
		o << std::boolalpha;
		auto it = container.begin();
		auto end = container.end();
		while (true) {
			o << *it;
			++it;
			if (it != end)
				o << delimiter;
			else
				return o.str();
		}
	}

	// =============
	// Header String
	// =============


	template<typename TContainer>
	inline std::string FormatUtil::to_header_string(const std::string& header, const TContainer& container, const std::string& indent)
	{
		std::ostringstream o;
		o << std::boolalpha;
		o << header;

		for (const auto& item : container) {
			o << std::endl;
			o << indent << item;
		}

		return o.str();
	}

	template<typename TMap>
	inline std::string FormatUtil::to_unsorted_map_string(const std::string& header, const TMap& map, const std::string& indent)
	{
		std::ostringstream o;
		o << std::boolalpha;
		o << header;

		for (const auto& it : map) {
			o << std::endl;
			o << indent << it->first << ": " << it->second;
		}

		return o.str();
	}

	template<typename TMap>
	inline std::string FormatUtil::to_map_string(const std::string& header, const TMap& map, const std::string& indent)
	{
		using TIterator = decltype(map.begin());

		std::vector<TIterator> v;
		for (auto iterator = map.begin(); iterator != map.end(); ++iterator)
			v.push_back(iterator);
		std::sort(v.begin(), v.end(), [](const TIterator& x, const TIterator& y) { return x->first < y->first; });

		std::ostringstream o;
		o << std::boolalpha;
		o << header;

		for (const auto& it : v) {
			o << std::endl;
			o << indent << it->first << ": " << it->second;
		}

		return o.str();
	}


	// =================
	// Formatted Strings
	// =================


	template<typename TFirst, typename... TTail>
	inline std::string FormatUtil::format_to_string(std::string format, const TFirst& first, const TTail& ... tail)
	{
		std::ostringstream o;
		o << std::boolalpha;
		_format_recursive(o, format, first, tail...);
		return o.str();
	}

	template<typename TFirst, typename... TTail>
	inline std::string FormatUtil::formatln_to_string(std::string format, const TFirst& first, const TTail& ... tail)
	{
		std::ostringstream o;
		o << std::boolalpha;
		_format_recursive(o, format, first, tail...);
		o << std::endl;
		return o.str();
	}

	template<typename TFirst, typename... TTail>
	inline void FormatUtil::format_to(std::ostream& output, std::string format, const TFirst& first, const TTail& ... tail)
	{
		_format_recursive(output, format, first, tail...);
	}

	template<typename TFirst, typename... TTail>
	inline void FormatUtil::formatln_to(std::ostream& output, std::string format, const TFirst& first, const TTail& ... tail)
	{
		_format_recursive(output, format, first, tail...);
		output << std::endl;
	}


	// ========
	// Printing
	// ========


	template<typename TFirst, typename... TTail>
	inline void FormatUtil::print(const std::string& format, const TFirst& first, const TTail& ... tail)
	{
		format_to(std::cout, format, first, tail...);
	}

	template<typename TFirst, typename... TTail>
	inline void FormatUtil::println(const std::string& format, const TFirst& first, const TTail& ... tail)
	{
		print(format, first, tail...);
		std::cout << std::endl;
	}

	inline void FormatUtil::print(const std::string& str)
	{
		std::cout << str;
	}

	inline void FormatUtil::println(const std::string& str)
	{
		std::cout << str << std::endl;
	}


	// ======================
	// Private Implementation
	// ======================


	template<typename TFirst, typename... TTail>
	inline void FormatUtil::_format_recursive(std::ostream& output, std::string& s, const TFirst& first, const TTail& ... tail)
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

} // namespace nova


#endif //NOVA_FORMAT_UTIL_HPP
